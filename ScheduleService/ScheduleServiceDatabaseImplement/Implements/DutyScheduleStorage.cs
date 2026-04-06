using Microsoft.EntityFrameworkCore;
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
    public class DutyScheduleStorage : IDutyScheduleStorage
    {
        private readonly ScheduleServiceDatabase _context;

        public DutyScheduleStorage(ScheduleServiceDatabase context)
        {
            _context = context;
        }

        public List<DutyScheduleViewModel> GetFullList()
        {
            return _context.DutySchedules
                .Include(x => x.DutyPerson)
                .Include(x => x.LessonTime)
                .Select(x => CreateModel(x))
                .ToList();
        }

        public List<DutyScheduleViewModel> GetFilteredList(DutyScheduleSearchModel model)
        {
            if (model == null)
            {
                return new();
            }

            var query = _context.DutySchedules
                .Include(x => x.DutyPerson)
                .Include(x => x.LessonTime)
                .AsQueryable();

            if (model.Id.HasValue)
            {
                query = query.Where(x => x.Id == model.Id.Value);
            }

            if (model.DateFrom.HasValue)
            {
                query = query.Where(x => x.Date >= model.DateFrom.Value);
            }

            if (model.DateTo.HasValue)
            {
                query = query.Where(x => x.Date <= model.DateTo.Value);
            }

            if (model.DutyPersonId.HasValue)
            {
                query = query.Where(x => x.DutyPersonId == model.DutyPersonId.Value);
            }

            if (model.LessonTimeId.HasValue)
            {
                query = query.Where(x => x.LessonTimeId == model.LessonTimeId.Value);
            }

            return query
                .Select(x => CreateModel(x))
                .ToList();
        }

        public DutyScheduleViewModel? GetElement(DutyScheduleSearchModel model)
        {
            if (model == null)
            {
                return null;
            }

            var entity = _context.DutySchedules
                .Include(x => x.DutyPerson)
                .Include(x => x.LessonTime)
                .FirstOrDefault(x => model.Id.HasValue && x.Id == model.Id.Value);

            return entity != null ? CreateModel(entity) : null;
        }

        public DutyScheduleViewModel? Insert(DutyScheduleBindingModel model)
        {
            var entity = new DutySchedule
            {
                Date = DateTime.SpecifyKind(model.Date.Date, DateTimeKind.Utc),
                LessonTimeId = model.LessonTimeId,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Place = model.Place,
                Comment = model.Comment,
                DutyPersonId = model.DutyPersonId
            };

            _context.DutySchedules.Add(entity);
            _context.SaveChanges();

            return GetElement(new DutyScheduleSearchModel { Id = entity.Id });
        }

        public DutyScheduleViewModel? Update(DutyScheduleBindingModel model)
        {
            var entity = _context.DutySchedules.FirstOrDefault(x => x.Id == model.Id);
            if (entity == null)
            {
                return null;
            }

            entity.Date = model.Date;
            entity.LessonTimeId = model.LessonTimeId;
            entity.StartTime = model.StartTime;
            entity.EndTime = model.EndTime;
            entity.Place = model.Place;
            entity.Comment = model.Comment;
            entity.DutyPersonId = model.DutyPersonId;

            _context.SaveChanges();

            return GetElement(new DutyScheduleSearchModel { Id = entity.Id });
        }

        public DutyScheduleViewModel? Delete(DutyScheduleBindingModel model)
        {
            var entity = _context.DutySchedules
                .Include(x => x.DutyPerson)
                .Include(x => x.LessonTime)
                .FirstOrDefault(x => x.Id == model.Id);

            if (entity == null)
            {
                return null;
            }

            var result = CreateModel(entity);
            _context.DutySchedules.Remove(entity);
            _context.SaveChanges();

            return result;
        }

        private static DutyScheduleViewModel CreateModel(DutySchedule entity)
        {
            return new DutyScheduleViewModel
            {
                Id = entity.Id,
                Date = entity.Date,
                LessonTimeId = entity.LessonTimeId,
                PairNumber = entity.LessonTime?.PairNumber,
                StartTime = entity.LessonTimeId.HasValue ? entity.LessonTime?.StartTime : entity.StartTime,
                EndTime = entity.LessonTimeId.HasValue ? entity.LessonTime?.EndTime : entity.EndTime,
                Place = entity.Place,
                Comment = entity.Comment,
                DutyPersonId = entity.DutyPersonId,
                DutyPersonName = entity.DutyPerson != null
                    ? $"{entity.DutyPerson.LastName} {entity.DutyPerson.FirstName}".Trim()
                    : string.Empty
            };
        }
    }
}
