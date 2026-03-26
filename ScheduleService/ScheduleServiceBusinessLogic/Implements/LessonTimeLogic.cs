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
    public class LessonTimeLogic : ILessonTimeLogic
    {
        private readonly ILessonTimeStorage _lessonTimeStorage;

        public LessonTimeLogic(ILessonTimeStorage lessonTimeStorage)
        {
            _lessonTimeStorage = lessonTimeStorage;
        }

        public List<LessonTimeViewModel>? ReadList(LessonTimeSearchModel? model)
        {
            return model == null
                ? _lessonTimeStorage.GetFullList()
                : _lessonTimeStorage.GetFilteredList(model);
        }

        public LessonTimeViewModel? ReadElement(LessonTimeSearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return _lessonTimeStorage.GetElement(model);
        }

        public LessonTimeViewModel? Create(LessonTimeBindingModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.PairNumber <= 0)
            {
                throw new ArgumentException("Некорректный номер пары");
            }

            if (model.StartTime >= model.EndTime)
            {
                throw new ArgumentException("Время начала должно быть меньше времени окончания");
            }

            var existing = _lessonTimeStorage.GetElement(new LessonTimeSearchModel
            {
                PairNumber = model.PairNumber
            });

            if (existing != null)
            {
                throw new InvalidOperationException("Пара с таким номером уже существует");
            }

            return _lessonTimeStorage.Insert(model);
        }

        public LessonTimeViewModel? Update(LessonTimeBindingModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentException("Не указан идентификатор времени пары");
            }

            if (model.PairNumber <= 0)
            {
                throw new ArgumentException("Некорректный номер пары");
            }

            if (model.StartTime >= model.EndTime)
            {
                throw new ArgumentException("Время начала должно быть меньше времени окончания");
            }

            var element = _lessonTimeStorage.GetElement(new LessonTimeSearchModel
            {
                Id = model.Id
            });

            if (element == null)
            {
                throw new InvalidOperationException("Время пары не найдено");
            }

            var existing = _lessonTimeStorage.GetElement(new LessonTimeSearchModel
            {
                PairNumber = model.PairNumber
            });

            if (existing != null && existing.Id != model.Id)
            {
                throw new InvalidOperationException("Другая пара с таким номером уже существует");
            }

            return _lessonTimeStorage.Update(model);
        }

        public bool Delete(LessonTimeBindingModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentException("Не указан идентификатор времени пары");
            }

            var element = _lessonTimeStorage.GetElement(new LessonTimeSearchModel
            {
                Id = model.Id
            });

            if (element == null)
            {
                throw new InvalidOperationException("Время пары не найдено");
            }

            return _lessonTimeStorage.Delete(model) != null;
        }
    }
}
