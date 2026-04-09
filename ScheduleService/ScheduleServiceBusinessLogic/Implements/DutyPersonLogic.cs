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

            NormalizeModel(model);
            ValidateModel(model);
            CheckDuplicates(model);

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

            NormalizeModel(model);
            ValidateModel(model);

            var element = _dutyPersonStorage.GetElement(new DutyPersonSearchModel
            {
                Id = model.Id
            });

            if (element == null)
            {
                throw new InvalidOperationException("Дежурный не найден");
            }

            CheckDuplicates(model);

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

        private static void NormalizeModel(DutyPersonBindingModel model)
        {
            model.LastName = model.LastName?.Trim() ?? string.Empty;
            model.FirstName = model.FirstName?.Trim() ?? string.Empty;
            model.Position = string.IsNullOrWhiteSpace(model.Position) ? null : model.Position.Trim();
            model.Phone = string.IsNullOrWhiteSpace(model.Phone) ? null : model.Phone.Trim();
            model.Email = string.IsNullOrWhiteSpace(model.Email) ? null : model.Email.Trim();
        }

        private static void ValidateModel(DutyPersonBindingModel model)
        {
            if (string.IsNullOrWhiteSpace(model.LastName))
            {
                throw new ArgumentException("Не указана фамилия дежурного");
            }

            if (string.IsNullOrWhiteSpace(model.FirstName))
            {
                throw new ArgumentException("Не указано имя дежурного");
            }
        }

        private void CheckDuplicates(DutyPersonBindingModel model)
        {
            var sameName = _dutyPersonStorage.GetElement(new DutyPersonSearchModel
            {
                LastName = model.LastName,
                FirstName = model.FirstName
            });

            if (sameName != null && sameName.Id != model.Id)
            {
                throw new InvalidOperationException("Дежурный с такими фамилией и именем уже существует");
            }

            if (!string.IsNullOrWhiteSpace(model.Phone))
            {
                var samePhone = _dutyPersonStorage.GetElement(new DutyPersonSearchModel
                {
                    Phone = model.Phone
                });

                if (samePhone != null && samePhone.Id != model.Id)
                {
                    throw new InvalidOperationException("Дежурный с таким телефоном уже существует");
                }
            }

            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                var sameEmail = _dutyPersonStorage.GetElement(new DutyPersonSearchModel
                {
                    Email = model.Email
                });

                if (sameEmail != null && sameEmail.Id != model.Id)
                {
                    throw new InvalidOperationException("Дежурный с таким email уже существует");
                }
            }
        }
    }
}
