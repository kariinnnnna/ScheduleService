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
    public class DutyScheduleLogic : IDutyScheduleLogic
    {
        private readonly IDutyScheduleStorage _dutyScheduleStorage;
        private readonly IDutyPersonStorage _dutyPersonStorage;
        private readonly ILessonTimeStorage _lessonTimeStorage;

        public DutyScheduleLogic(
            IDutyScheduleStorage dutyScheduleStorage,
            IDutyPersonStorage dutyPersonStorage,
            ILessonTimeStorage lessonTimeStorage)
        {
            _dutyScheduleStorage = dutyScheduleStorage;
            _dutyPersonStorage = dutyPersonStorage;
            _lessonTimeStorage = lessonTimeStorage;
        }

        public List<DutyScheduleViewModel>? ReadList(DutyScheduleSearchModel? model)
        {
            return model == null
                ? _dutyScheduleStorage.GetFullList()
                : _dutyScheduleStorage.GetFilteredList(model);
        }

        public DutyScheduleViewModel? ReadElement(DutyScheduleSearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return _dutyScheduleStorage.GetElement(model);
        }

        public DutyScheduleViewModel? Create(DutyScheduleBindingModel model)
        {
            ValidateModel(model);

            return _dutyScheduleStorage.Insert(model);
        }

        public DutyScheduleViewModel? Update(DutyScheduleBindingModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentException("Не указан идентификатор записи дежурства");
            }

            var element = _dutyScheduleStorage.GetElement(new DutyScheduleSearchModel
            {
                Id = model.Id
            });

            if (element == null)
            {
                throw new InvalidOperationException("Запись дежурства не найдена");
            }

            ValidateModel(model);

            return _dutyScheduleStorage.Update(model);
        }

        public bool Delete(DutyScheduleBindingModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentException("Не указан идентификатор записи дежурства");
            }

            var element = _dutyScheduleStorage.GetElement(new DutyScheduleSearchModel
            {
                Id = model.Id
            });

            if (element == null)
            {
                throw new InvalidOperationException("Запись дежурства не найдена");
            }

            return _dutyScheduleStorage.Delete(model) != null;
        }

        private void ValidateModel(DutyScheduleBindingModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Date == default)
            {
                throw new ArgumentException("Не указана дата");
            }

            if (!model.LessonTimeId.HasValue || model.LessonTimeId.Value <= 0)
            {
                throw new ArgumentException("Не указано время пары");
            }

            if (model.DutyPersonId <= 0)
            {
                throw new ArgumentException("Не указан дежурный");
            }

            var dutyPerson = _dutyPersonStorage.GetElement(new DutyPersonSearchModel
            {
                Id = model.DutyPersonId
            });

            if (dutyPerson == null)
            {
                throw new InvalidOperationException("Указанный дежурный не найден");
            }

            var lessonTime = _lessonTimeStorage.GetElement(new LessonTimeSearchModel
            {
                Id = model.LessonTimeId.Value
            });

            if (lessonTime == null)
            {
                throw new InvalidOperationException("Указанное время пары не найдено");
            }
        }
    }
}
