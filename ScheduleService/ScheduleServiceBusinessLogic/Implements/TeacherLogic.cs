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
    public class TeacherLogic : ITeacherLogic
    {
        private readonly ITeacherStorage _teacherStorage;

        public TeacherLogic(ITeacherStorage teacherStorage)
        {
            _teacherStorage = teacherStorage;
        }

        public List<TeacherViewModel>? ReadList(TeacherSearchModel? model)
        {
            return model == null
                ? _teacherStorage.GetFullList()
                : _teacherStorage.GetFilteredList(model);
        }

        public TeacherViewModel? ReadElement(TeacherSearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return _teacherStorage.GetElement(model);
        }

        public TeacherViewModel? Create(TeacherBindingModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrWhiteSpace(model.TeacherName))
            {
                throw new ArgumentException("Не указано имя преподавателя");
            }

            var existingByCoreId = _teacherStorage.GetElement(new TeacherSearchModel
            {
                CoreSystemId = model.CoreSystemId
            });

            if (existingByCoreId != null)
            {
                throw new InvalidOperationException("Преподаватель с таким CoreSystemId уже существует");
            }

            return _teacherStorage.Insert(model);
        }

        public TeacherViewModel? Update(TeacherBindingModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentException("Не указан идентификатор преподавателя");
            }

            if (string.IsNullOrWhiteSpace(model.TeacherName))
            {
                throw new ArgumentException("Не указано имя преподавателя");
            }

            var element = _teacherStorage.GetElement(new TeacherSearchModel
            {
                Id = model.Id
            });

            if (element == null)
            {
                throw new InvalidOperationException("Преподаватель не найден");
            }

            var existingByCoreId = _teacherStorage.GetElement(new TeacherSearchModel
            {
                CoreSystemId = model.CoreSystemId
            });

            if (existingByCoreId != null && existingByCoreId.Id != model.Id)
            {
                throw new InvalidOperationException("Другой преподаватель с таким CoreSystemId уже существует");
            }

            return _teacherStorage.Update(model);
        }

        public bool Delete(TeacherBindingModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentException("Не указан идентификатор преподавателя");
            }

            var element = _teacherStorage.GetElement(new TeacherSearchModel
            {
                Id = model.Id
            });

            if (element == null)
            {
                throw new InvalidOperationException("Преподаватель не найден");
            }

            return _teacherStorage.Delete(model) != null;
        }
    }
}
