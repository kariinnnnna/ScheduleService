using HtmlAgilityPack;
using ScheduleServiceContracts.BindingModels;
using ScheduleServiceContracts.BusinessLogicContracts;
using ScheduleServiceContracts.SearchModels;
using ScheduleServiceContracts.StorageContracts;
using ScheduleServiceContracts.ViewModels;
using ScheduleServiceDataModels.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace ScheduleServiceBusinessLogic.Implements
{
    public class UniversityScheduleLogic : IUniversityScheduleLogic
    {
        private readonly IScheduleItemStorage _scheduleItemStorage;
        private readonly ILessonTimeStorage _lessonTimeStorage;

        public UniversityScheduleLogic(
            IScheduleItemStorage scheduleItemStorage,
            ILessonTimeStorage lessonTimeStorage)
        {
            _scheduleItemStorage = scheduleItemStorage;
            _lessonTimeStorage = lessonTimeStorage;
        }

        public ParsedUniversityScheduleViewModel ParseGroupSchedule(UniversityScheduleParseBindingModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (string.IsNullOrWhiteSpace(model.Url))
                throw new ArgumentException("Не указан URL страницы расписания.");

            if (string.IsNullOrWhiteSpace(model.CookieHeader))
                throw new ArgumentException("Не указаны cookies авторизованной сессии.");

            var html = LoadHtml(model.Url, model.CookieHeader);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            return ParseGroupScheduleDocument(doc);
        }

        public ParsedUniversityScheduleViewModel ParseGroupScheduleFromHtml(UniversityScheduleParseHtmlBindingModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (string.IsNullOrWhiteSpace(model.HtmlContent))
                throw new ArgumentException("HTML-код страницы не передан.");

            var doc = new HtmlDocument();
            doc.LoadHtml(model.HtmlContent);

            return ParseGroupScheduleDocument(doc);
        }

        public void ImportGroupSchedulesFromFolder(UniversityScheduleImportFolderBindingModel model)
        {
            _scheduleItemStorage.DeleteImported();
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (string.IsNullOrWhiteSpace(model.FolderPath))
                throw new ArgumentException("Не указан путь к папке.");

            if (!Directory.Exists(model.FolderPath))
                throw new DirectoryNotFoundException("Указанная папка не найдена.");

            var files = Directory.GetFiles(model.FolderPath, "*.html", SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
                throw new InvalidOperationException("В папке не найдено HTML-файлов.");

            foreach (var file in files)
            {
                var html = File.ReadAllText(file);
                var parsed = ParseGroupScheduleFromHtml(new UniversityScheduleParseHtmlBindingModel
                {
                    HtmlContent = html
                });

                foreach (var item in parsed.Items)
                {
                    var lessonTimeId = TryResolveLessonTimeId(item.PairNumber);
                    var (startTime, endTime) = ParseTimeRange(item.TimeRange);

                    _scheduleItemStorage.Insert(new ScheduleItemBindingModel
                    {
                        Type = MapLessonType(item.LessonType),
                        Date = DateTime.SpecifyKind(item.Date, DateTimeKind.Utc),
                        Subject = item.Subject,
                        LessonTimeId = lessonTimeId,
                        StartTime = lessonTimeId.HasValue ? null : startTime,
                        EndTime = lessonTimeId.HasValue ? null : endTime,
                        ClassroomNumber = string.IsNullOrWhiteSpace(item.ClassroomNumber) ? "Не указана" : item.ClassroomNumber,
                        GroupName = string.IsNullOrWhiteSpace(item.GroupName) ? parsed.GroupName : item.GroupName,
                        TeacherName = string.IsNullOrWhiteSpace(item.TeacherName) ? "Не указан" : item.TeacherName,
                        Comment = string.IsNullOrWhiteSpace(item.Subgroup) ? null : item.Subgroup,
                        IsImported = true
                    });
                }
            }
        }

        private ParsedUniversityScheduleViewModel ParseGroupScheduleDocument(HtmlDocument doc)
{
    var result = new ParsedUniversityScheduleViewModel();

    ParseHeader(doc, result);

    var scheduleTables = GetScheduleTables(doc);

    foreach (var scheduleTable in scheduleTables)
    {
        var rows = scheduleTable.SelectNodes(".//tr");
        if (rows == null || rows.Count < 3)
            continue;

        var pairNumbers = ExtractPairNumbers(rows[0]);
        var pairTimes = ExtractPairTimes(rows[1]);

        for (int rowIndex = 2; rowIndex < rows.Count; rowIndex++)
        {
            var cells = rows[rowIndex].SelectNodes("./th|./td");
            if (cells == null || cells.Count < 2)
                continue;

            var dayCellText = NormalizeText(cells[0].InnerText);
            var parsedDay = TryParseDayCell(dayCellText);
            if (parsedDay == null)
                continue;

            var (dayName, date) = parsedDay.Value;

            for (int colIndex = 1; colIndex < cells.Count; colIndex++)
            {
                var rawCellText = NormalizeCellText(cells[colIndex]);
                if (string.IsNullOrWhiteSpace(rawCellText))
                    continue;

                var pairNumber = colIndex <= pairNumbers.Count ? pairNumbers[colIndex - 1] : colIndex;
                var timeRange = colIndex <= pairTimes.Count ? pairTimes[colIndex - 1] : string.Empty;

                var parsedItems = ParseLessonsFromCell(
                    rawCellText,
                    result.GroupName,
                    dayName,
                    date,
                    pairNumber,
                    timeRange);

                result.Items.AddRange(parsedItems);
            }
        }
    }

    return result;
}

        private string LoadHtml(string url, string cookieHeader)
        {
            using var handler = new HttpClientHandler
            {
                AllowAutoRedirect = true
            };

            using var client = new HttpClient(handler);

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Cookie", cookieHeader);
            request.Headers.Add("User-Agent", "Mozilla/5.0");
            request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            request.Headers.Add("Accept-Language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
            request.Headers.Referrer = new Uri("https://lk.ulstu.ru/timetable/");

            var response = client.Send(request);
            var html = response.Content.ReadAsStringAsync().Result;

            if (!response.IsSuccessStatusCode)
            {
                var preview = html.Length > 1000 ? html.Substring(0, 1000) : html;

                throw new Exception(
                    $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}\n" +
                    $"Final URL: {response.RequestMessage?.RequestUri}\n" +
                    $"Preview:\n{preview}");
            }

            return html;
        }

        private void ParseHeader(HtmlDocument doc, ParsedUniversityScheduleViewModel result)
        {
            var rawText = HtmlEntity.DeEntitize(doc.DocumentNode.InnerText);
            rawText = rawText.Replace("\r", "\n");
            rawText = Regex.Replace(rawText, @"[ \t]+", " ");
            rawText = Regex.Replace(rawText, @"\n{2,}", "\n");

            var groupMatch = Regex.Match(
                rawText,
                @"Расписание занятий учебной группы:\s*([^\r\n]+?)(?=\s*Неделя:)",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            if (!groupMatch.Success)
            {
                groupMatch = Regex.Match(
                    rawText,
                    @"Расписание занятий учебной группы:\s*([^\r\n]+)",
                    RegexOptions.IgnoreCase);
            }

            if (groupMatch.Success)
            {
                result.GroupName = Regex.Replace(groupMatch.Groups[1].Value, @"\s+", " ").Trim();
            }

            var weekMatch = Regex.Match(
                rawText,
                @"Неделя:\s*(\d+)-я\s*\((\d{2}\.\d{2}\.\d{4})\s*[—\-]\s*(\d{2}\.\d{2}\.\d{4})\)",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            if (weekMatch.Success)
            {
                result.WeekNumber = int.Parse(weekMatch.Groups[1].Value);
                result.WeekStartDate = DateTime.ParseExact(weekMatch.Groups[2].Value, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                result.WeekEndDate = DateTime.ParseExact(weekMatch.Groups[3].Value, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            }
        }

        private List<HtmlNode> GetScheduleTables(HtmlDocument doc)
        {
            var result = new List<HtmlNode>();

            var tables = doc.DocumentNode.SelectNodes("//table");
            if (tables == null || tables.Count == 0)
                throw new InvalidOperationException("На странице не найдены таблицы.");

            foreach (var table in tables)
            {
                var rows = table.SelectNodes(".//tr");
                if (rows == null || rows.Count < 3)
                    continue;

                var firstRowText = NormalizeText(rows[0].InnerText);
                var secondRowText = rows.Count > 1 ? NormalizeText(rows[1].InnerText) : string.Empty;

                if (firstRowText.Contains("Пары", StringComparison.OrdinalIgnoreCase) &&
                    secondRowText.Contains("Время", StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(table);
                }
            }

            if (result.Count == 0)
                throw new InvalidOperationException("Не удалось найти таблицы расписания с заголовками 'Пары' и 'Время'.");

            return result;
        }

        private List<int> ExtractPairNumbers(HtmlNode row)
        {
            var result = new List<int>();
            var cells = row.SelectNodes("./th|./td");

            if (cells == null || cells.Count < 2)
                return result;

            for (int i = 1; i < cells.Count; i++)
            {
                var text = NormalizeText(cells[i].InnerText);
                var match = Regex.Match(text, @"(\d+)");
                if (match.Success)
                {
                    result.Add(int.Parse(match.Groups[1].Value));
                }
            }

            return result;
        }

        private List<string> ExtractPairTimes(HtmlNode row)
        {
            var result = new List<string>();
            var cells = row.SelectNodes("./th|./td");

            if (cells == null || cells.Count < 2)
                return result;

            for (int i = 1; i < cells.Count; i++)
            {
                result.Add(NormalizeText(cells[i].InnerText));
            }

            return result;
        }

        private (string DayName, DateTime Date)? TryParseDayCell(string text)
        {
            var match = Regex.Match(
                text,
                @"^([А-Яа-яA-Za-zёЁ]+),?\s*(\d{2}\.\d{2}\.\d{4})",
                RegexOptions.IgnoreCase);

            if (!match.Success)
                return null;

            var dayName = match.Groups[1].Value.Trim();
            var date = DateTime.ParseExact(
                match.Groups[2].Value,
                "dd.MM.yyyy",
                CultureInfo.InvariantCulture);

            return (dayName, date);
        }

        private string NormalizeCellText(HtmlNode cell)
        {
            var html = cell.InnerHtml;

            html = Regex.Replace(html, @"<br\s*/?>", "\n", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"</p\s*>", "\n", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"</div\s*>", "\n", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"</font\s*>", "\n", RegexOptions.IgnoreCase);

            var tempDoc = new HtmlDocument();
            tempDoc.LoadHtml(html);

            var text = HtmlEntity.DeEntitize(tempDoc.DocumentNode.InnerText);
            text = text.Replace("\r", "\n");
            text = Regex.Replace(text, @"\n[ \t]+", "\n");
            text = Regex.Replace(text, @"[ \t]+", " ");
            text = Regex.Replace(text, @"\n{2,}", "\n");

            return text.Trim();
        }

        private string NormalizeText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            text = HtmlEntity.DeEntitize(text);
            text = text.Replace("\r", " ").Replace("\n", " ");
            text = Regex.Replace(text, @"\s+", " ");
            return text.Trim();
        }

        private List<ParsedUniversityScheduleItemViewModel> ParseLessonsFromCell(
            string rawCellText,
            string groupName,
            string dayName,
            DateTime date,
            int pairNumber,
            string timeRange)
        {
            var result = new List<ParsedUniversityScheduleItemViewModel>();

            var lines = rawCellText
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            if (lines.Count == 0)
                return result;

            var lessonTypeIndexes = new List<int>();
            for (int i = 0; i < lines.Count; i++)
            {
                if (IsLessonType(lines[i]))
                {
                    lessonTypeIndexes.Add(i);
                }
            }

            if (lessonTypeIndexes.Count == 0)
            {
                var parsed = ParseSingleLessonBlock(lines);

                parsed.GroupName = groupName;
                parsed.DayName = dayName;
                parsed.Date = date;
                parsed.PairNumber = pairNumber;
                parsed.TimeRange = timeRange;
                parsed.RawCellText = rawCellText;

                result.Add(parsed);
                return result;
            }


            for (int i = 0; i < lessonTypeIndexes.Count; i++)
            {
                var startIndex = lessonTypeIndexes[i];
                var endIndex = (i + 1 < lessonTypeIndexes.Count)
                    ? lessonTypeIndexes[i + 1]
                    : lines.Count;

                var block = lines.GetRange(startIndex, endIndex - startIndex);
                var parsed = ParseSingleLessonBlock(block);

                parsed.GroupName = groupName;
                parsed.DayName = dayName;
                parsed.Date = date;
                parsed.PairNumber = pairNumber;
                parsed.TimeRange = timeRange;
                parsed.RawCellText = string.Join("\n", block);

                result.Add(parsed);
            }

            return result;
        }

        private ParsedUniversityScheduleItemViewModel ParseSingleLessonBlock(List<string> block)
        {
            var item = new ParsedUniversityScheduleItemViewModel();

            if (block.Count == 0)
                return item;

            item.LessonType = block[0];

            if (block.Count >= 2)
                item.Subject = block[1];

            for (int i = 2; i < block.Count; i++)
            {
                var line = block[i];

                if (IsSubgroup(line))
                {
                    item.Subgroup = line;
                    continue;
                }

                if (IsClassroom(line))
                {
                    item.ClassroomNumber = line;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(item.TeacherName))
                {
                    item.TeacherName = line;
                }
            }

            return item;
        }

        private bool IsLessonType(string value)
        {
            var normalized = value.Trim().ToLower().Replace(" ", "");

            return normalized is "лек."
                or "лек"
                or "пр."
                or "пр"
                or "лаб."
                or "лаб"
                or "экз."
                or "экз"
                or "зач."
                or "зач"
                or "зач.о"
                or "зач.о."
                or "конс."
                or "конс"
                or "курс.пр."
                or "курс.пр"
                or "курс.раб."
                or "курс.раб";
        }

        private bool IsSubgroup(string value)
        {
            var normalized = value.Trim().ToLower();
            return normalized.Contains("п/г");
        }

        private bool IsClassroom(string value)
        {
            var text = value.Trim().Replace('–', '-');

            return Regex.IsMatch(text, @"^\d+\-\d+[А-Яа-яA-Za-z]?$")          // 6-604
                || Regex.IsMatch(text, @"^\d+_[0-9А-Яа-яA-Za-z]+$")           // 3_3, 3_5
                || Regex.IsMatch(text, @"^\d+\-\d+\-\d+$")                    // запасной вариант
                || Regex.IsMatch(text, @"^\d+\-[А-Яа-яA-Za-z]{1,3}$")         // 2-СЗ
                || Regex.IsMatch(text, @"^\d+\-\d+\/\d+$")                    // 3-424/2
                || Regex.IsMatch(text, @"^\d+\-\d+[А-Яа-яA-Za-z]?\/\d+$");    // редкий смешанный вариант
        }

        private int? TryResolveLessonTimeId(int pairNumber)
        {
            var lessonTime = _lessonTimeStorage.GetElement(new LessonTimeSearchModel
            {
                PairNumber = pairNumber
            });

            return lessonTime?.Id;
        }

        private (TimeSpan? StartTime, TimeSpan? EndTime) ParseTimeRange(string timeRange)
        {
            if (string.IsNullOrWhiteSpace(timeRange))
                return (null, null);

            var parts = timeRange
                .Replace("–", "-")
                .Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (parts.Length != 2)
                return (null, null);

            if (TimeSpan.TryParse(parts[0], out var start) &&
                TimeSpan.TryParse(parts[1], out var end))
            {
                return (start, end);
            }

            return (null, null);
        }

        private ScheduleItemType MapLessonType(string lessonType)
        {
            var normalized = lessonType.Trim().ToLower().Replace(" ", "");

            return normalized switch
            {
                "лек." or "лек" => ScheduleItemType.Lecture,
                "пр." or "пр" => ScheduleItemType.Practice,
                "лаб." or "лаб" => ScheduleItemType.Laboratory,
                "конс." or "конс" => ScheduleItemType.Consultation,
                "экз." or "экз" => ScheduleItemType.Exam,
                "зач." or "зач" or "зач.о" or "зач.о." => ScheduleItemType.Test,
                _ => ScheduleItemType.Practice
            };
        }
    }
}