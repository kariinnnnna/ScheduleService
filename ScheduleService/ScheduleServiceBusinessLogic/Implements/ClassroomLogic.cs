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
    public class ClassroomLogic : IClassroomLogic
    {
        private readonly IClassroomStorage _classroomStorage;

        public ClassroomLogic(IClassroomStorage classroomStorage)
        {
            _classroomStorage = classroomStorage;
        }

        public List<ClassroomViewModel>? ReadList(ClassroomSearchModel? model)
        {
            return model == null
                ? _classroomStorage.GetFullList()
                : _classroomStorage.GetFilteredList(model);
        }

        public ClassroomViewModel? ReadElement(ClassroomSearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return _classroomStorage.GetElement(model);
        }

        public ClassroomViewModel? Create(ClassroomBindingModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrWhiteSpace(model.Number))
            {
                throw new ArgumentException("Не указан номер аудитории");
            }

            var existingByCoreId = _classroomStorage.GetElement(new ClassroomSearchModel
            {
                CoreSystemId = model.CoreSystemId
            });

            if (existingByCoreId != null)
            {
                throw new InvalidOperationException("Аудитория с таким CoreSystemId уже существует");
            }

            return _classroomStorage.Insert(model);
        }

        public ClassroomViewModel? Update(ClassroomBindingModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentException("Не указан идентификатор аудитории");
            }

            if (string.IsNullOrWhiteSpace(model.Number))
            {
                throw new ArgumentException("Не указан номер аудитории");
            }

            var element = _classroomStorage.GetElement(new ClassroomSearchModel
            {
                Id = model.Id
            });

            if (element == null)
            {
                throw new InvalidOperationException("Аудитория не найдена");
            }

            var existingByCoreId = _classroomStorage.GetElement(new ClassroomSearchModel
            {
                CoreSystemId = model.CoreSystemId
            });

            if (existingByCoreId != null && existingByCoreId.Id != model.Id)
            {
                throw new InvalidOperationException("Другая аудитория с таким CoreSystemId уже существует");
            }

            return _classroomStorage.Update(model);
        }

        public bool Delete(ClassroomBindingModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentException("Не указан идентификатор аудитории");
            }

            var element = _classroomStorage.GetElement(new ClassroomSearchModel
            {
                Id = model.Id
            });

            if (element == null)
            {
                throw new InvalidOperationException("Аудитория не найдена");
            }

            return _classroomStorage.Delete(model) != null;
        }
    }
}
