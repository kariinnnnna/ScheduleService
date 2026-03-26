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
    public class DutyPersonLogic : IDutyPersonLogic
    {
        private readonly IDutyPersonStorage _dutyPersonStorage;

        public DutyPersonLogic(IDutyPersonStorage dutyPersonStorage)
        {
            _dutyPersonStorage = dutyPersonStorage;
        }

        public List<DutyPersonViewModel>? ReadList(DutyPersonSearchModel? model)
        {
            return model == null
                ? _dutyPersonStorage.GetFullList()
                : _dutyPersonStorage.GetFilteredList(model);
        }

        public DutyPersonViewModel? ReadElement(DutyPersonSearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return _dutyPersonStorage.GetElement(model);
        }

        public DutyPersonViewModel? Create(DutyPersonBindingModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrWhiteSpace(model.LastName))
            {
                throw new ArgumentException("Не указана фамилия дежурного");
            }

            if (string.IsNullOrWhiteSpace(model.FirstName))
            {
                throw new ArgumentException("Не указано имя дежурного");
            }

            return _dutyPersonStorage.Insert(model);
        }

        public DutyPersonViewModel? Update(DutyPersonBindingModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentException("Не указан идентификатор дежурного");
            }

            if (string.IsNullOrWhiteSpace(model.LastName))
            {
                throw new ArgumentException("Не указана фамилия дежурного");
            }

            if (string.IsNullOrWhiteSpace(model.FirstName))
            {
                throw new ArgumentException("Не указано имя дежурного");
            }

            var element = _dutyPersonStorage.GetElement(new DutyPersonSearchModel
            {
                Id = model.Id
            });

            if (element == null)
            {
                throw new InvalidOperationException("Дежурный не найден");
            }

            return _dutyPersonStorage.Update(model);
        }

        public bool Delete(DutyPersonBindingModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentException("Не указан идентификатор дежурного");
            }

            var element = _dutyPersonStorage.GetElement(new DutyPersonSearchModel
            {
                Id = model.Id
            });

            if (element == null)
            {
                throw new InvalidOperationException("Дежурный не найден");
            }

            return _dutyPersonStorage.Delete(model) != null;
        }
    }
}
