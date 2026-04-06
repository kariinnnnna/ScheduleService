using ScheduleServiceContracts.BindingModels;
using ScheduleServiceContracts.BusinessLogicContracts;
using ScheduleServiceContracts.SearchModels;
using ScheduleServiceContracts.StorageContracts;
using ScheduleServiceContracts.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceBusinessLogic.Implements
{
    public class ScheduleItemLogic : IScheduleItemLogic
    {
        private readonly IScheduleItemStorage _scheduleItemStorage;
        private readonly IGroupStorage _groupStorage;
        private readonly ITeacherStorage _teacherStorage;
        private readonly ILessonTimeStorage _lessonTimeStorage;

        public ScheduleItemLogic(
            IScheduleItemStorage scheduleItemStorage,
            IGroupStorage groupStorage,
            ITeacherStorage teacherStorage,
            ILessonTimeStorage lessonTimeStorage)
        {
            _scheduleItemStorage = scheduleItemStorage;
            _groupStorage = groupStorage;
            _teacherStorage = teacherStorage;
            _lessonTimeStorage = lessonTimeStorage;
        }

        public List<ScheduleItemViewModel>? ReadList(ScheduleItemSearchModel? model)
        {
            return model == null
                ? _scheduleItemStorage.GetFullList()
                : _scheduleItemStorage.GetFilteredList(model);
        }

        public ScheduleItemViewModel? ReadElement(ScheduleItemSearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return _scheduleItemStorage.GetElement(model);
        }

        public ScheduleItemViewModel? Create(ScheduleItemBindingModel model)
        {
            ValidateModel(model);
            ValidateClassroomAvailability(model);

            return _scheduleItemStorage.Insert(model);
        }

        public ScheduleItemViewModel? Update(ScheduleItemBindingModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentException("Не указан идентификатор записи расписания");
            }

            var element = _scheduleItemStorage.GetElement(new ScheduleItemSearchModel
            {
                Id = model.Id
            });

            if (element == null)
            {
                throw new InvalidOperationException("Запись расписания не найдена");
            }

            ValidateModel(model);
            ValidateClassroomAvailability(model);

            return _scheduleItemStorage.Update(model);
        }

        public bool Delete(ScheduleItemBindingModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentException("Не указан идентификатор записи расписания");
            }

            var element = _scheduleItemStorage.GetElement(new ScheduleItemSearchModel
            {
                Id = model.Id
            });

            if (element == null)
            {
                throw new InvalidOperationException("Запись расписания не найдена");
            }

            return _scheduleItemStorage.Delete(model) != null;
        }

        private void ValidateModel(ScheduleItemBindingModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrWhiteSpace(model.Subject))
            {
                throw new ArgumentException("Не указана дисциплина");
            }

            if (model.LessonTimeId.HasValue)
            {
                var lessonTime = _lessonTimeStorage.GetElement(new LessonTimeSearchModel
                {
                    Id = model.LessonTimeId.Value
                });

                if (lessonTime == null)
                {
                    throw new InvalidOperationException("Указанное время пары не найдено");
                }
            }
            else
            {
                if (!model.StartTime.HasValue || !model.EndTime.HasValue)
                {
                    throw new ArgumentException("Нужно указать время начала и окончания");
                }

                if (model.StartTime.Value >= model.EndTime.Value)
                {
                    throw new ArgumentException("Время начала должно быть меньше времени окончания");
                }
            }

            if (model.ClassroomId.HasValue)
            {
                if (model.ClassroomId <= 0)
                {
                    throw new Exception("Некорректная аудитория");
                }
            }
            else if (string.IsNullOrWhiteSpace(model.ClassroomNumber))
            {
                throw new ArgumentException("Не указана аудитория");
            }

            if (model.GroupId.HasValue)
            {
                var group = _groupStorage.GetElement(new GroupSearchModel
                {
                    Id = model.GroupId.Value
                });

                if (group == null)
                {
                    throw new InvalidOperationException("Указанная группа не найдена");
                }
            }
            else if (string.IsNullOrWhiteSpace(model.GroupName))
            {
                throw new ArgumentException("Не указана группа");
            }

            if (model.TeacherId.HasValue)
            {
                var teacher = _teacherStorage.GetElement(new TeacherSearchModel
                {
                    Id = model.TeacherId.Value
                });

                if (teacher == null)
                {
                    throw new InvalidOperationException("Указанный преподаватель не найден");
                }
            }
            else if (string.IsNullOrWhiteSpace(model.TeacherName))
            {
                throw new ArgumentException("Не указан преподаватель");
            }
        }
        private void ValidateClassroomAvailability(ScheduleItemBindingModel model)
        {
            var allItems = _scheduleItemStorage.GetFullList() ?? new List<ScheduleItemViewModel>();

            var currentStart = GetStartTime(model);
            var currentEnd = GetEndTime(model);

            var sameDateItems = allItems.Where(x => x.Date.Date == model.Date.Date);

            IEnumerable<ScheduleItemViewModel> sameClassroomItems;

            if (model.ClassroomId.HasValue)
            {
                sameClassroomItems = sameDateItems.Where(x => x.ClassroomId == model.ClassroomId.Value);
            }
            else
            {
                var classroomNumber = (model.ClassroomNumber ?? string.Empty).Trim();
                sameClassroomItems = sameDateItems.Where(x =>
                    !string.IsNullOrWhiteSpace(x.ClassroomNumber) &&
                    x.ClassroomNumber.Trim().Equals(classroomNumber, StringComparison.OrdinalIgnoreCase));
            }

            if (model.Id > 0)
            {
                sameClassroomItems = sameClassroomItems.Where(x => x.Id != model.Id);
            }

            foreach (var item in sameClassroomItems)
            {
                var existingStart = item.StartTime;
                var existingEnd = item.EndTime;

                if (!existingStart.HasValue || !existingEnd.HasValue)
                    continue;

                var isIntersected = currentStart < existingEnd.Value && currentEnd > existingStart.Value;

                if (isIntersected)
                {
                    var classroomText = !string.IsNullOrWhiteSpace(item.ClassroomNumber)
                        ? item.ClassroomNumber
                        : "указанной аудитории";

                    throw new InvalidOperationException(
                        $"Аудитория {classroomText} уже занята на {model.Date:dd.MM.yyyy} " +
                        $"с {existingStart:hh\\:mm} до {existingEnd:hh\\:mm}.");
                }
            }
        }
        private TimeSpan GetStartTime(ScheduleItemBindingModel model)
        {
            if (model.LessonTimeId.HasValue)
            {
                var lessonTime = _lessonTimeStorage.GetElement(new LessonTimeSearchModel
                {
                    Id = model.LessonTimeId.Value
                });

                if (lessonTime == null)
                {
                    throw new InvalidOperationException("Не удалось определить время начала пары.");
                }

                return lessonTime.StartTime;
            }

            if (!model.StartTime.HasValue)
            {
                throw new ArgumentException("Не указано время начала.");
            }

            return model.StartTime.Value;
        }

        private TimeSpan GetEndTime(ScheduleItemBindingModel model)
        {
            if (model.LessonTimeId.HasValue)
            {
                var lessonTime = _lessonTimeStorage.GetElement(new LessonTimeSearchModel
                {
                    Id = model.LessonTimeId.Value
                });

                if (lessonTime == null)
                {
                    throw new InvalidOperationException("Не удалось определить время окончания пары.");
                }

                return lessonTime.EndTime;
            }

            if (!model.EndTime.HasValue)
            {
                throw new ArgumentException("Не указано время окончания.");
            }

            return model.EndTime.Value;
        }
    }
}
