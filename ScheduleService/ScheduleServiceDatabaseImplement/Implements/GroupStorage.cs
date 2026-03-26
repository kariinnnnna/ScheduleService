using ScheduleServiceContracts.BindingModels;
using ScheduleServiceContracts.SearchModels;
using ScheduleServiceContracts.StorageContracts;
using ScheduleServiceContracts.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroupEntity = ScheduleServiceDatabaseImplement.Models.Group;

namespace ScheduleServiceDatabaseImplement.Implements
{
    public class GroupStorage : IGroupStorage
    {
        private readonly ScheduleServiceDatabase _context;

        public GroupStorage(ScheduleServiceDatabase context)
        {
            _context = context;
        }

        public List<GroupViewModel> GetFullList()
        {
            return _context.Groups
                .Select(x => CreateModel(x))
                .ToList();
        }

        public List<GroupViewModel> GetFilteredList(GroupSearchModel model)
        {
            if (model == null)
            {
                return new();
            }

            var query = _context.Groups.AsQueryable();

            if (model.Id.HasValue)
            {
                query = query.Where(x => x.Id == model.Id.Value);
            }

            if (model.CoreSystemId.HasValue)
            {
                query = query.Where(x => x.CoreSystemId == model.CoreSystemId.Value);
            }

            if (!string.IsNullOrWhiteSpace(model.GroupName))
            {
                query = query.Where(x => x.GroupName.Contains(model.GroupName));
            }

            return query
                .Select(x => CreateModel(x))
                .ToList();
        }

        public GroupViewModel? GetElement(GroupSearchModel model)
        {
            if (model == null)
            {
                return null;
            }

            var entity = _context.Groups.FirstOrDefault(x =>
                (model.Id.HasValue && x.Id == model.Id.Value) ||
                (model.CoreSystemId.HasValue && x.CoreSystemId == model.CoreSystemId.Value));

            return entity != null ? CreateModel(entity) : null;
        }

        public GroupViewModel? Insert(GroupBindingModel model)
        {
            var entity = new GroupEntity
            {
                CoreSystemId = model.CoreSystemId,
                GroupName = model.GroupName
            };

            _context.Groups.Add(entity);
            _context.SaveChanges();

            return CreateModel(entity);
        }

        public GroupViewModel? Update(GroupBindingModel model)
        {
            var entity = _context.Groups.FirstOrDefault(x => x.Id == model.Id);
            if (entity == null)
            {
                return null;
            }

            entity.CoreSystemId = model.CoreSystemId;
            entity.GroupName = model.GroupName;

            _context.SaveChanges();

            return CreateModel(entity);
        }

        public GroupViewModel? Delete(GroupBindingModel model)
        {
            var entity = _context.Groups.FirstOrDefault(x => x.Id == model.Id);
            if (entity == null)
            {
                return null;
            }

            var result = CreateModel(entity);
            _context.Groups.Remove(entity);
            _context.SaveChanges();

            return result;
        }

        private static GroupViewModel CreateModel(GroupEntity entity)
        {
            return new GroupViewModel
            {
                Id = entity.Id,
                CoreSystemId = entity.CoreSystemId,
                GroupName = entity.GroupName
            };
        }
    }
}