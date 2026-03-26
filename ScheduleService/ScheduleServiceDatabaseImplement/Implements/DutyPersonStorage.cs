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
    public class DutyPersonStorage : IDutyPersonStorage
    {
        private readonly ScheduleServiceDatabase _context;

        public DutyPersonStorage(ScheduleServiceDatabase context)
        {
            _context = context;
        }

        public List<DutyPersonViewModel> GetFullList()
        {
            return _context.DutyPersons
                .Select(x => CreateModel(x))
                .ToList();
        }

        public List<DutyPersonViewModel> GetFilteredList(DutyPersonSearchModel model)
        {
            if (model == null)
            {
                return new();
            }

            var query = _context.DutyPersons.AsQueryable();

            if (model.Id.HasValue)
            {
                query = query.Where(x => x.Id == model.Id.Value);
            }

            if (!string.IsNullOrWhiteSpace(model.LastName))
            {
                query = query.Where(x => x.LastName.Contains(model.LastName));
            }

            if (!string.IsNullOrWhiteSpace(model.FirstName))
            {
                query = query.Where(x => x.FirstName.Contains(model.FirstName));
            }

            if (!string.IsNullOrWhiteSpace(model.Position))
            {
                query = query.Where(x => x.Position != null && x.Position.Contains(model.Position));
            }

            return query
                .Select(x => CreateModel(x))
                .ToList();
        }

        public DutyPersonViewModel? GetElement(DutyPersonSearchModel model)
        {
            if (model == null)
            {
                return null;
            }

            var entity = _context.DutyPersons.FirstOrDefault(x => model.Id.HasValue && x.Id == model.Id.Value);

            return entity != null ? CreateModel(entity) : null;
        }

        public DutyPersonViewModel? Insert(DutyPersonBindingModel model)
        {
            var entity = new DutyPerson
            {
                LastName = model.LastName,
                FirstName = model.FirstName,
                Position = model.Position,
                Phone = model.Phone,
                Email = model.Email
            };

            _context.DutyPersons.Add(entity);
            _context.SaveChanges();

            return CreateModel(entity);
        }

        public DutyPersonViewModel? Update(DutyPersonBindingModel model)
        {
            var entity = _context.DutyPersons.FirstOrDefault(x => x.Id == model.Id);
            if (entity == null)
            {
                return null;
            }

            entity.LastName = model.LastName;
            entity.FirstName = model.FirstName;
            entity.Position = model.Position;
            entity.Phone = model.Phone;
            entity.Email = model.Email;

            _context.SaveChanges();

            return CreateModel(entity);
        }

        public DutyPersonViewModel? Delete(DutyPersonBindingModel model)
        {
            var entity = _context.DutyPersons.FirstOrDefault(x => x.Id == model.Id);
            if (entity == null)
            {
                return null;
            }

            var result = CreateModel(entity);
            _context.DutyPersons.Remove(entity);
            _context.SaveChanges();

            return result;
        }

        private static DutyPersonViewModel CreateModel(DutyPerson entity)
        {
            return new DutyPersonViewModel
            {
                Id = entity.Id,
                LastName = entity.LastName,
                FirstName = entity.FirstName,
                Position = entity.Position,
                Phone = entity.Phone,
                Email = entity.Email
            };
        }
    }
}
