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
    public class LessonTimeStorage : ILessonTimeStorage
    {
        private readonly ScheduleServiceDatabase _context;

        public LessonTimeStorage(ScheduleServiceDatabase context)
        {
            _context = context;
        }

        public List<LessonTimeViewModel> GetFullList()
        {
            return _context.LessonTimes
                .OrderBy(x => x.PairNumber)
                .Select(x => CreateModel(x))
                .ToList();
        }

        public List<LessonTimeViewModel> GetFilteredList(LessonTimeSearchModel model)
        {
            if (model == null)
            {
                return new();
            }

            var query = _context.LessonTimes.AsQueryable();

            if (model.Id.HasValue)
            {
                query = query.Where(x => x.Id == model.Id.Value);
            }

            if (model.PairNumber.HasValue)
            {
                query = query.Where(x => x.PairNumber == model.PairNumber.Value);
            }

            return query
                .OrderBy(x => x.PairNumber)
                .Select(x => CreateModel(x))
                .ToList();
        }

        public LessonTimeViewModel? GetElement(LessonTimeSearchModel model)
        {
            if (model == null)
            {
                return null;
            }

            var entity = _context.LessonTimes.FirstOrDefault(x =>
                (model.Id.HasValue && x.Id == model.Id.Value) ||
                (model.PairNumber.HasValue && x.PairNumber == model.PairNumber.Value));

            return entity != null ? CreateModel(entity) : null;
        }

        public LessonTimeViewModel? Insert(LessonTimeBindingModel model)
        {
            var entity = new LessonTime
            {
                PairNumber = model.PairNumber,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Description = model.Description
            };

            _context.LessonTimes.Add(entity);
            _context.SaveChanges();

            return CreateModel(entity);
        }

        public LessonTimeViewModel? Update(LessonTimeBindingModel model)
        {
            var entity = _context.LessonTimes.FirstOrDefault(x => x.Id == model.Id);
            if (entity == null)
            {
                return null;
            }

            entity.PairNumber = model.PairNumber;
            entity.StartTime = model.StartTime;
            entity.EndTime = model.EndTime;
            entity.Description = model.Description;

            _context.SaveChanges();

            return CreateModel(entity);
        }

        public LessonTimeViewModel? Delete(LessonTimeBindingModel model)
        {
            var entity = _context.LessonTimes.FirstOrDefault(x => x.Id == model.Id);
            if (entity == null)
            {
                return null;
            }

            var result = CreateModel(entity);
            _context.LessonTimes.Remove(entity);
            _context.SaveChanges();

            return result;
        }

        private static LessonTimeViewModel CreateModel(LessonTime entity)
        {
            return new LessonTimeViewModel
            {
                Id = entity.Id,
                PairNumber = entity.PairNumber,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                Description = entity.Description
            };
        }
    }
}
