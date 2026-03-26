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
    public class ClassroomStorage : IClassroomStorage
    {
        private readonly ScheduleServiceDatabase _context;

        public ClassroomStorage(ScheduleServiceDatabase context)
        {
            _context = context;
        }

        public List<ClassroomViewModel> GetFullList()
        {
            return _context.Classrooms
                .Select(x => CreateModel(x))
                .ToList();
        }

        public List<ClassroomViewModel> GetFilteredList(ClassroomSearchModel model)
        {
            if (model == null)
            {
                return new();
            }

            var query = _context.Classrooms.AsQueryable();

            if (model.Id.HasValue)
            {
                query = query.Where(x => x.Id == model.Id.Value);
            }

            if (model.CoreSystemId.HasValue)
            {
                query = query.Where(x => x.CoreSystemId == model.CoreSystemId.Value);
            }

            if (!string.IsNullOrWhiteSpace(model.Number))
            {
                query = query.Where(x => x.Number.Contains(model.Number));
            }

            if (model.NotUseInSchedule.HasValue)
            {
                query = query.Where(x => x.NotUseInSchedule == model.NotUseInSchedule.Value);
            }

            return query
                .Select(x => CreateModel(x))
                .ToList();
        }

        public ClassroomViewModel? GetElement(ClassroomSearchModel model)
        {
            if (model == null)
            {
                return null;
            }

            var entity = _context.Classrooms.FirstOrDefault(x =>
                (model.Id.HasValue && x.Id == model.Id.Value) ||
                (model.CoreSystemId.HasValue && x.CoreSystemId == model.CoreSystemId.Value));

            return entity != null ? CreateModel(entity) : null;
        }

        public ClassroomViewModel? Insert(ClassroomBindingModel model)
        {
            var entity = new Classroom
            {
                CoreSystemId = model.CoreSystemId,
                Number = model.Number,
                Type = model.Type,
                Capacity = model.Capacity,
                NotUseInSchedule = model.NotUseInSchedule
            };

            _context.Classrooms.Add(entity);
            _context.SaveChanges();

            return CreateModel(entity);
        }

        public ClassroomViewModel? Update(ClassroomBindingModel model)
        {
            var entity = _context.Classrooms.FirstOrDefault(x => x.Id == model.Id);
            if (entity == null)
            {
                return null;
            }

            entity.CoreSystemId = model.CoreSystemId;
            entity.Number = model.Number;
            entity.Type = model.Type;
            entity.Capacity = model.Capacity;
            entity.NotUseInSchedule = model.NotUseInSchedule;

            _context.SaveChanges();

            return CreateModel(entity);
        }

        public ClassroomViewModel? Delete(ClassroomBindingModel model)
        {
            var entity = _context.Classrooms.FirstOrDefault(x => x.Id == model.Id);
            if (entity == null)
            {
                return null;
            }

            var result = CreateModel(entity);
            _context.Classrooms.Remove(entity);
            _context.SaveChanges();

            return result;
        }

        private static ClassroomViewModel CreateModel(Classroom entity)
        {
            return new ClassroomViewModel
            {
                Id = entity.Id,
                CoreSystemId = entity.CoreSystemId,
                Number = entity.Number,
                Type = entity.Type,
                Capacity = entity.Capacity,
                NotUseInSchedule = entity.NotUseInSchedule
            };
        }
    }
}
