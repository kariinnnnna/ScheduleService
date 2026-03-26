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
    public class GroupLogic : IGroupLogic
    {
        private readonly IGroupStorage _groupStorage;

        public GroupLogic(IGroupStorage groupStorage)
        {
            _groupStorage = groupStorage;
        }

        public List<GroupViewModel>? ReadList(GroupSearchModel? model)
        {
            return model == null
                ? _groupStorage.GetFullList()
                : _groupStorage.GetFilteredList(model);
        }

        public GroupViewModel? ReadElement(GroupSearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return _groupStorage.GetElement(model);
        }

        public GroupViewModel? Create(GroupBindingModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrWhiteSpace(model.GroupName))
            {
                throw new ArgumentException("Не указано название группы");
            }

            var existingByCoreId = _groupStorage.GetElement(new GroupSearchModel
            {
                CoreSystemId = model.CoreSystemId
            });

            if (existingByCoreId != null)
            {
                throw new InvalidOperationException("Группа с таким CoreSystemId уже существует");
            }

            return _groupStorage.Insert(model);
        }

        public GroupViewModel? Update(GroupBindingModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentException("Не указан идентификатор группы");
            }

            if (string.IsNullOrWhiteSpace(model.GroupName))
            {
                throw new ArgumentException("Не указано название группы");
            }

            var element = _groupStorage.GetElement(new GroupSearchModel
            {
                Id = model.Id
            });

            if (element == null)
            {
                throw new InvalidOperationException("Группа не найдена");
            }

            var existingByCoreId = _groupStorage.GetElement(new GroupSearchModel
            {
                CoreSystemId = model.CoreSystemId
            });

            if (existingByCoreId != null && existingByCoreId.Id != model.Id)
            {
                throw new InvalidOperationException("Другая группа с таким CoreSystemId уже существует");
            }

            return _groupStorage.Update(model);
        }

        public bool Delete(GroupBindingModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentException("Не указан идентификатор группы");
            }

            var element = _groupStorage.GetElement(new GroupSearchModel
            {
                Id = model.Id
            });

            if (element == null)
            {
                throw new InvalidOperationException("Группа не найдена");
            }

            return _groupStorage.Delete(model) != null;
        }
    }
}
