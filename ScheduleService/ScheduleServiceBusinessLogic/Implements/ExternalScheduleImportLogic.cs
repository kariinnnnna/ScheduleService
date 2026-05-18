using ScheduleServiceBusinessLogic.Helpers;
using ScheduleServiceContracts.BindingModels;
using ScheduleServiceContracts.BusinessLogicContracts;
using ScheduleServiceContracts.SearchModels;
using ScheduleServiceContracts.StorageContracts;
using ScheduleServiceContracts.ViewModels;
using ScheduleServiceDataModels.Enums;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace ScheduleServiceBusinessLogic.Implements
{
    public class ExternalScheduleImportLogic : IExternalScheduleImportLogic
    {
        private readonly ExternalScheduleApiService _externalScheduleApiService;
        private readonly IScheduleItemStorage _scheduleItemStorage;
        private readonly IScheduleItemLogic _scheduleItemLogic;
        private readonly ILessonTimeStorage _lessonTimeStorage;
        private readonly IExternalScheduleSyncStateStorage _syncStateStorage;

        public ExternalScheduleImportLogic(
            ExternalScheduleApiService externalScheduleApiService,
            IScheduleItemStorage scheduleItemStorage,
            IScheduleItemLogic scheduleItemLogic,
            ILessonTimeStorage lessonTimeStorage,
            IExternalScheduleSyncStateStorage syncStateStorage)
        {
            _externalScheduleApiService = externalScheduleApiService;
            _scheduleItemStorage = scheduleItemStorage;
            _scheduleItemLogic = scheduleItemLogic;
            _lessonTimeStorage = lessonTimeStorage;
            _syncStateStorage = syncStateStorage;
        }

        public async Task<ExternalScheduleImportResultViewModel> ImportAsync(
            ExternalScheduleImportBindingModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.ClassroomNumbers == null || model.ClassroomNumbers.Count == 0)
            {
                throw new ArgumentException("Не передан список аудиторий кафедры.");
            }

            var result = new ExternalScheduleImportResultViewModel();

            var departmentClassrooms = model.ClassroomNumbers
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(NormalizeClassroomNumber)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (departmentClassrooms.Count == 0)
            {
                throw new ArgumentException("Список аудиторий кафедры пуст.");
            }

            var currentClassroomNumbersHash =
                CalculateClassroomNumbersHash(model.ClassroomNumbers);

            const string jobName = "ExternalScheduleImport";

            var currentVersion = await _externalScheduleApiService.GetLastVersionAsync();

            result.CurrentVersionId = currentVersion.Id;

            var previousState = _syncStateStorage.GetElement(
                new ExternalScheduleSyncStateSearchModel
                {
                    JobName = jobName
                });

            result.PreviousVersionId = previousState?.LastVersionId;

            if (!model.ForceImport &&
                previousState != null &&
                previousState.LastVersionId == currentVersion.Id &&
                string.Equals(
                    previousState.ClassroomNumbersHash,
                    currentClassroomNumbersHash,
                    StringComparison.OrdinalIgnoreCase))
            {
                result.SkippedByVersion = true;
                result.Message =
                    $"Расписание и список аудиторий не изменились. " +
                    $"Текущая версия: {currentVersion.Id}. Импорт не выполнялся.";

                return result;
            }

            var currentWeek = await _externalScheduleApiService.GetCurrentWeekAsync();

            // Важно: current-week из API соответствует текущей учебной неделе,
            // поэтому базовую дату берём от сегодняшнего дня, а не от даты,
            // выбранной пользователем на странице расписания.
            var baseDate = DateTime.Today.Date;
            var currentMonday = GetMonday(baseDate);

            var groups = await _externalScheduleApiService.GetGroupsAsync();

            groups = groups
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList();

            result.TotalGroupsCount = groups.Count;

            var filteredLessons = new List<ExternalLessonWithDate>();

            foreach (var group in groups)
            {
                try
                {
                    var lessons = await _externalScheduleApiService.GetTimetableAsync(group);

                    result.ProcessedGroupsCount++;
                    result.ReceivedLessonsCount += lessons.Count;

                    var lessonsInDepartmentClassrooms = lessons
                        .Where(x => departmentClassrooms.Contains(
                            NormalizeClassroomNumber(x.ClassroomNumber)))
                        .ToList();

                    result.FilteredByClassroomCount += lessonsInDepartmentClassrooms.Count;

                    foreach (var lesson in lessonsInDepartmentClassrooms)
                    {
                        var date = CalculateLessonDate(
                            currentMonday,
                            currentWeek,
                            lesson.StudyWeek,
                            lesson.Day);

                        filteredLessons.Add(new ExternalLessonWithDate
                        {
                            Date = date,
                            PairNumber = lesson.PairNumber,
                            GroupName = lesson.GroupName,
                            TeacherName = lesson.TeacherName,
                            ClassroomNumber = lesson.ClassroomNumber,
                            LessonName = lesson.LessonName
                        });
                    }
                }
                catch (Exception ex)
                {
                    result.ErrorCount++;
                    result.Errors.Add($"Группа {group}: {ex.Message}");
                }
            }

            if (result.ErrorCount > 0)
            {
                result.Message =
                    $"Импорт не применён: при получении расписания возникли ошибки. " +
                    $"Старое импортированное расписание сохранено. " +
                    $"Количество ошибок: {result.ErrorCount}.";

                return result;
            }

            var groupedLessons = GroupLessons(filteredLessons);
            result.GroupedLessonsCount = groupedLessons.Count;

            var lessonTimes = _lessonTimeStorage.GetFullList()
                ?? new List<LessonTimeViewModel>();

            _scheduleItemStorage.DeleteImported();

            if (groupedLessons.Count == 0)
            {
                SaveSyncState(
                    jobName,
                    currentVersion.Id,
                    currentVersion.UpdateDate,
                    currentClassroomNumbersHash);

                result.Message =
                    $"Импорт выполнен. Занятий в аудиториях кафедры не найдено. " +
                    $"Версия расписания: {currentVersion.Id}.";

                return result;
            }

            foreach (var lesson in groupedLessons)
            {
                try
                {
                    var lessonTime = lessonTimes
                        .FirstOrDefault(x => x.PairNumber == lesson.PairNumber);

                    if (lessonTime == null)
                    {
                        result.SkippedCount++;
                        result.Errors.Add(
                            $"Не найдена пара №{lesson.PairNumber}. " +
                            $"{lesson.Date:dd.MM.yyyy}, {lesson.ClassroomNumber}, {lesson.LessonName}");

                        continue;
                    }

                    var parsedLesson = ParseLessonName(lesson.LessonName);

                    var bindingModel = new ScheduleItemBindingModel
                    {
                        Type = parsedLesson.Type,
                        Date = DateTime.SpecifyKind(lesson.Date.Date, DateTimeKind.Utc),

                        LessonTimeId = lessonTime.Id,
                        StartTime = null,
                        EndTime = null,

                        ClassroomId = null,
                        ClassroomNumber = lesson.ClassroomNumber,

                        TeacherId = null,
                        TeacherName = lesson.TeacherName,

                        GroupId = null,
                        GroupName = string.Join(
                            ", ",
                            lesson.GroupNames.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)),

                        Subject = parsedLesson.Subject,
                        Comment = parsedLesson.Comment,

                        IsImported = true
                    };

                    _scheduleItemLogic.Create(bindingModel);

                    result.CreatedCount++;
                }
                catch (Exception ex)
                {
                    result.SkippedCount++;
                    result.Errors.Add(
                        $"{lesson.Date:dd.MM.yyyy}, {lesson.ClassroomNumber}, " +
                        $"{lesson.LessonName}: {ex.Message}");
                }
            }

            if (result.SkippedCount > 0)
            {
                result.Message =
                    $"Импорт выполнен с пропусками. " +
                    $"Версия {currentVersion.Id} не сохранена как полностью синхронизированная. " +
                    $"При следующем запуске будет выполнена повторная попытка.";

                return result;
            }

            SaveSyncState(
                jobName,
                currentVersion.Id,
                currentVersion.UpdateDate,
                currentClassroomNumbersHash);

            result.Message = $"Импорт выполнен. Версия расписания: {currentVersion.Id}.";

            return result;
        }

        private void SaveSyncState(
            string jobName,
            int versionId,
            string updateDate,
            string classroomNumbersHash)
        {
            _syncStateStorage.InsertOrUpdate(new ExternalScheduleSyncStateBindingModel
            {
                JobName = jobName,
                LastVersionId = versionId,
                LastUpdateDate = ParseExternalUpdateDate(updateDate),
                LastSyncDate = DateTime.UtcNow,
                ClassroomNumbersHash = classroomNumbersHash
            });
        }

        private static string NormalizeClassroomNumber(string? value)
        {
            return (value ?? string.Empty)
                .Trim()
                .Replace(" ", string.Empty)
                .ToLowerInvariant();
        }

        private static string Normalize(string? value)
        {
            return (value ?? string.Empty)
                .Trim()
                .ToLowerInvariant();
        }

        private static DateTime GetMonday(DateTime date)
        {
            var dayOfWeek = (int)date.DayOfWeek;

            var daysFromMonday = dayOfWeek == 0
                ? 6
                : dayOfWeek - 1;

            return date.Date.AddDays(-daysFromMonday);
        }

        private static DateTime CalculateLessonDate(
            DateTime currentMonday,
            int currentStudyWeek,
            int lessonStudyWeek,
            int lessonDay)
        {
            var weekOffset = lessonStudyWeek - currentStudyWeek;

            return currentMonday
                .AddDays(weekOffset * 7)
                .AddDays(lessonDay)
                .Date;
        }

        private static List<GroupedExternalLesson> GroupLessons(
            List<ExternalLessonWithDate> lessons)
        {
            return lessons
                .GroupBy(x => new
                {
                    x.Date,
                    x.PairNumber,
                    ClassroomNumber = NormalizeClassroomNumber(x.ClassroomNumber),
                    TeacherName = Normalize(x.TeacherName),
                    LessonName = Normalize(x.LessonName)
                })
                .Select(group =>
                {
                    var first = group.First();

                    return new GroupedExternalLesson
                    {
                        Date = first.Date,
                        PairNumber = first.PairNumber,
                        ClassroomNumber = first.ClassroomNumber,
                        TeacherName = first.TeacherName,
                        LessonName = first.LessonName,
                        GroupNames = group
                            .Select(x => x.GroupName)
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .ToList()
                    };
                })
                .OrderBy(x => x.Date)
                .ThenBy(x => x.PairNumber)
                .ThenBy(x => x.ClassroomNumber)
                .ToList();
        }

        private static ParsedLesson ParseLessonName(string lessonName)
        {
            var text = (lessonName ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(text))
            {
                return new ParsedLesson
                {
                    Type = ScheduleItemType.Consultation,
                    Subject = "Занятие",
                    Comment = string.Empty
                };
            }

            var lower = text.ToLowerInvariant();

            var type = ScheduleItemType.Consultation;
            var subject = text;

            if (lower.StartsWith("лек."))
            {
                type = ScheduleItemType.Lecture;
                subject = text.Substring(4).Trim();
            }
            else if (lower.StartsWith("лаб."))
            {
                type = ScheduleItemType.Laboratory;
                subject = text.Substring(4).Trim();
            }
            else if (lower.StartsWith("пр."))
            {
                type = ScheduleItemType.Practice;
                subject = text.Substring(3).Trim();
            }
            else if (lower.StartsWith("прак."))
            {
                type = ScheduleItemType.Practice;
                subject = text.Substring(5).Trim();
            }
            else if (lower.StartsWith("зач."))
            {
                type = ScheduleItemType.Test;
                subject = text.Substring(4).Trim();
            }
            else if (lower.StartsWith("экз."))
            {
                type = ScheduleItemType.Exam;
                subject = text.Substring(4).Trim();
            }
            else if (lower.StartsWith("конс."))
            {
                type = ScheduleItemType.Consultation;
                subject = text.Substring(5).Trim();
            }

            var comment = string.Empty;
            var separators = new[] { " – ", " - " };

            foreach (var separator in separators)
            {
                var index = subject.IndexOf(separator, StringComparison.Ordinal);

                if (index > 0)
                {
                    comment = subject[(index + separator.Length)..].Trim();
                    subject = subject[..index].Trim();
                    break;
                }
            }

            if (string.IsNullOrWhiteSpace(subject))
            {
                subject = text;
            }

            return new ParsedLesson
            {
                Type = type,
                Subject = subject,
                Comment = comment
            };
        }

        private static DateTime ParseExternalUpdateDate(string? updateDate)
        {
            if (string.IsNullOrWhiteSpace(updateDate))
            {
                return DateTime.UtcNow;
            }

            var value = updateDate.Trim();

            if (value.Length >= 5)
            {
                var tail = value[^5..];

                if ((tail[0] == '+' || tail[0] == '-') &&
                    tail.Skip(1).All(char.IsDigit))
                {
                    value = value[..^2] + ":" + value[^2..];
                }
            }

            if (DateTimeOffset.TryParse(
                    value,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal,
                    out var parsed))
            {
                return parsed.UtcDateTime;
            }

            return DateTime.UtcNow;
        }

        private static string CalculateClassroomNumbersHash(
            IEnumerable<string> classroomNumbers)
        {
            var normalizedClassrooms = classroomNumbers
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(NormalizeClassroomNumber)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var source = string.Join("|", normalizedClassrooms);

            using var sha256 = SHA256.Create();

            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(source));

            return Convert.ToHexString(bytes);
        }

        private class ExternalLessonWithDate
        {
            public DateTime Date { get; set; }
            public int PairNumber { get; set; }

            public string GroupName { get; set; } = string.Empty;
            public string TeacherName { get; set; } = string.Empty;
            public string ClassroomNumber { get; set; } = string.Empty;
            public string LessonName { get; set; } = string.Empty;
        }

        private class GroupedExternalLesson
        {
            public DateTime Date { get; set; }
            public int PairNumber { get; set; }

            public string ClassroomNumber { get; set; } = string.Empty;
            public string TeacherName { get; set; } = string.Empty;
            public string LessonName { get; set; } = string.Empty;

            public List<string> GroupNames { get; set; } = new();
        }

        private class ParsedLesson
        {
            public ScheduleItemType Type { get; set; }
            public string Subject { get; set; } = string.Empty;
            public string Comment { get; set; } = string.Empty;
        }
    }
}