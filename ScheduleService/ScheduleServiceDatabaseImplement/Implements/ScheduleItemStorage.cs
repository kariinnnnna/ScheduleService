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
    public class ScheduleItemStorage : IScheduleItemStorage
    {
        private readonly ScheduleServiceDatabase _context;

        public ScheduleItemStorage(ScheduleServiceDatabase context)
        {
            _context = context;
        }

        public List<ScheduleItemViewModel> GetFullList()
        {
            return _context.ScheduleItems
                .Include(x => x.Group)
                .Include(x => x.Teacher)
                .Include(x => x.LessonTime)
                .Select(x => CreateModel(x))
                .ToList();
        }

        public List<ScheduleItemViewModel> GetFilteredList(ScheduleItemSearchModel model)
        {
            if (model == null)
            {
                return new();
            }

            var query = _context.ScheduleItems
                .Include(x => x.Group)
                .Include(x => x.Teacher)
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

            if (model.Type.HasValue)
            {
                query = query.Where(x => x.Type == model.Type.Value);
            }

            if (model.LessonTimeId.HasValue)
            {
                query = query.Where(x => x.LessonTimeId == model.LessonTimeId.Value);
            }

            if (model.ClassroomId.HasValue)
            {
                query = query.Where(x => x.ClassroomId == model.ClassroomId.Value);
            }

            if (model.GroupId.HasValue)
            {
                query = query.Where(x => x.GroupId == model.GroupId.Value);
            }

            if (model.TeacherId.HasValue)
            {
                query = query.Where(x => x.TeacherId == model.TeacherId.Value);
            }

            if (!string.IsNullOrWhiteSpace(model.Subject))
            {
                query = query.Where(x => x.Subject.Contains(model.Subject));
            }

            return query
                .Select(x => CreateModel(x))
                .ToList();
        }

        public ScheduleItemViewModel? GetElement(ScheduleItemSearchModel model)
        {
            if (model == null)
            {
                return null;
            }

            var entity = _context.ScheduleItems
                .Include(x => x.Group)
                .Include(x => x.Teacher)
                .Include(x => x.LessonTime)
                .FirstOrDefault(x => model.Id.HasValue && x.Id == model.Id.Value);

            return entity != null ? CreateModel(entity) : null;
        }

        public ScheduleItemViewModel? Insert(ScheduleItemBindingModel model)
        {
            var entity = new ScheduleItem
            {
                Type = model.Type,
                Date = model.Date,
                Subject = model.Subject,
                LessonTimeId = model.LessonTimeId,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                ClassroomId = model.ClassroomId,
                ClassroomNumber = model.ClassroomNumber,
                GroupId = model.GroupId,
                GroupName = model.GroupName,
                TeacherId = model.TeacherId,
                TeacherName = model.TeacherName,
                Comment = model.Comment,
                IsImported = model.IsImported
            };

            _context.ScheduleItems.Add(entity);
            _context.SaveChanges();

            return GetElement(new ScheduleItemSearchModel { Id = entity.Id });
        }

        public ScheduleItemViewModel? Update(ScheduleItemBindingModel model)
        {
            var entity = _context.ScheduleItems.FirstOrDefault(x => x.Id == model.Id);
            if (entity == null)
            {
                return null;
            }

            entity.Type = model.Type;
            entity.Date = model.Date;
            entity.Subject = model.Subject;
            entity.LessonTimeId = model.LessonTimeId;
            entity.StartTime = model.StartTime;
            entity.EndTime = model.EndTime;
            entity.ClassroomId = model.ClassroomId;
            entity.ClassroomNumber = model.ClassroomNumber;
            entity.GroupId = model.GroupId;
            entity.GroupName = model.GroupName;
            entity.TeacherId = model.TeacherId;
            entity.TeacherName = model.TeacherName;
            entity.Comment = model.Comment;
            entity.IsImported = model.IsImported;

            _context.SaveChanges();

            return GetElement(new ScheduleItemSearchModel { Id = entity.Id });
        }

        public ScheduleItemViewModel? Delete(ScheduleItemBindingModel model)
        {
            var entity = _context.ScheduleItems
                .Include(x => x.Group)
                .Include(x => x.Teacher)
                .Include(x => x.LessonTime)
                .FirstOrDefault(x => x.Id == model.Id);

            if (entity == null)
            {
                return null;
            }

            var result = CreateModel(entity);
            _context.ScheduleItems.Remove(entity);
            _context.SaveChanges();

            return result;
        }

        private static ScheduleItemViewModel CreateModel(ScheduleItem entity)
        {
            return new ScheduleItemViewModel
            {
                Id = entity.Id,
                Type = entity.Type,
                Date = entity.Date,
                Subject = entity.Subject,
                LessonTimeId = entity.LessonTimeId,
                PairNumber = entity.LessonTime != null
    ? entity.LessonTime.PairNumber
    : GetPairNumberByTime(
        entity.LessonTimeId.HasValue ? entity.LessonTime?.StartTime : entity.StartTime,
        entity.LessonTimeId.HasValue ? entity.LessonTime?.EndTime : entity.EndTime),
                StartTime = entity.LessonTimeId.HasValue ? entity.LessonTime?.StartTime : entity.StartTime,
                EndTime = entity.LessonTimeId.HasValue ? entity.LessonTime?.EndTime : entity.EndTime,
                ClassroomId = entity.ClassroomId,
                ClassroomNumber = entity.ClassroomNumber,
                GroupId = entity.GroupId,
                GroupName = entity.Group != null ? entity.Group.GroupName : entity.GroupName,
                TeacherId = entity.TeacherId,
                TeacherName = entity.Teacher != null ? entity.Teacher.TeacherName : entity.TeacherName,
                Comment = entity.Comment,
                IsImported = entity.IsImported
            };
        }
        private static int? GetPairNumberByTime(TimeSpan? startTime, TimeSpan? endTime)
        {
            if (!startTime.HasValue || !endTime.HasValue)
                return null;

            if (startTime == TimeSpan.Parse("08:30") && endTime == TimeSpan.Parse("09:50")) return 1;
            if (startTime == TimeSpan.Parse("10:00") && endTime == TimeSpan.Parse("11:20")) return 2;
            if (startTime == TimeSpan.Parse("11:30") && endTime == TimeSpan.Parse("12:50")) return 3;
            if (startTime == TimeSpan.Parse("13:30") && endTime == TimeSpan.Parse("14:50")) return 4;
            if (startTime == TimeSpan.Parse("15:00") && endTime == TimeSpan.Parse("16:20")) return 5;
            if (startTime == TimeSpan.Parse("16:30") && endTime == TimeSpan.Parse("17:50")) return 6;
            if (startTime == TimeSpan.Parse("18:00") && endTime == TimeSpan.Parse("19:20")) return 7;
            if (startTime == TimeSpan.Parse("19:30") && endTime == TimeSpan.Parse("20:50")) return 8;

            return null;
        }
        public void DeleteImported()
        {
            var items = _context.ScheduleItems
                .Where(x => x.IsImported)
                .ToList();

            if (items.Count == 0)
                return;

            _context.ScheduleItems.RemoveRange(items);
            _context.SaveChanges();
        }
    }
}
