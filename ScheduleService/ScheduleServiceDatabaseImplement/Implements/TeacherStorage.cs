using ScheduleServiceContracts.BindingModels;
using ScheduleServiceContracts.SearchModels;
using ScheduleServiceContracts.StorageContracts;
using ScheduleServiceContracts.ViewModels;
using ScheduleServiceDatabaseImplement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceDatabaseImplement.Implements
{
    public class TeacherStorage : ITeacherStorage
    {
        private readonly ScheduleServiceDatabase _context;

        public TeacherStorage(ScheduleServiceDatabase context)
        {
            _context = context;
        }

        public List<TeacherViewModel> GetFullList()
        {
            return _context.Teachers
                .Select(x => CreateModel(x))
                .ToList();
        }

        public List<TeacherViewModel> GetFilteredList(TeacherSearchModel model)
        {
            if (model == null)
            {
                return new();
            }

            var query = _context.Teachers.AsQueryable();

            if (model.Id.HasValue)
            {
                query = query.Where(x => x.Id == model.Id.Value);
            }

            if (model.CoreSystemId.HasValue)
            {
                query = query.Where(x => x.CoreSystemId == model.CoreSystemId.Value);
            }

            if (!string.IsNullOrWhiteSpace(model.TeacherName))
            {
                query = query.Where(x => x.TeacherName.Contains(model.TeacherName));
            }

            return query
                .Select(x => CreateModel(x))
                .ToList();
        }

        public TeacherViewModel? GetElement(TeacherSearchModel model)
        {
            if (model == null)
            {
                return null;
            }

            var entity = _context.Teachers.FirstOrDefault(x =>
                (model.Id.HasValue && x.Id == model.Id.Value) ||
                (model.CoreSystemId.HasValue && x.CoreSystemId == model.CoreSystemId.Value));

            return entity != null ? CreateModel(entity) : null;
        }

        public TeacherViewModel? Insert(TeacherBindingModel model)
        {
            var entity = new Teacher
            {
                CoreSystemId = model.CoreSystemId,
                TeacherName = model.TeacherName
            };

            _context.Teachers.Add(entity);
            _context.SaveChanges();

            return CreateModel(entity);
        }

        public TeacherViewModel? Update(TeacherBindingModel model)
        {
            var entity = _context.Teachers.FirstOrDefault(x => x.Id == model.Id);
            if (entity == null)
            {
                return null;
            }

            entity.CoreSystemId = model.CoreSystemId;
            entity.TeacherName = model.TeacherName;

            _context.SaveChanges();

            return CreateModel(entity);
        }

        public TeacherViewModel? Delete(TeacherBindingModel model)
        {
            var entity = _context.Teachers.FirstOrDefault(x => x.Id == model.Id);
            if (entity == null)
            {
                return null;
            }

            var result = CreateModel(entity);
            _context.Teachers.Remove(entity);
            _context.SaveChanges();

            return result;
        }

        private static TeacherViewModel CreateModel(Teacher entity)
        {
            return new TeacherViewModel
            {
                Id = entity.Id,
                CoreSystemId = entity.CoreSystemId,
                TeacherName = entity.TeacherName
            };
        }
    }
}
