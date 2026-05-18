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
    public class ExternalScheduleSyncStateStorage : IExternalScheduleSyncStateStorage
    {
        private readonly ScheduleServiceDatabase _context;

        public ExternalScheduleSyncStateStorage(ScheduleServiceDatabase context)
        {
            _context = context;
        }

        public ExternalScheduleSyncStateViewModel? GetElement(ExternalScheduleSyncStateSearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var query = _context.ExternalScheduleSyncStates.AsQueryable();

            if (model.Id.HasValue)
            {
                query = query.Where(x => x.Id == model.Id.Value);
            }

            if (!string.IsNullOrWhiteSpace(model.JobName))
            {
                query = query.Where(x => x.JobName == model.JobName);
            }

            return query
            .Select(x => new ExternalScheduleSyncStateViewModel
            {
                Id = x.Id,
                JobName = x.JobName,
                LastVersionId = x.LastVersionId,
                LastUpdateDate = x.LastUpdateDate,
                LastSyncDate = x.LastSyncDate,
                ClassroomNumbersHash = x.ClassroomNumbersHash
            })
            .FirstOrDefault();
        }

        public ExternalScheduleSyncStateViewModel? InsertOrUpdate(ExternalScheduleSyncStateBindingModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrWhiteSpace(model.JobName))
            {
                throw new ArgumentException("Не указано имя задачи синхронизации");
            }

            var element = _context.ExternalScheduleSyncStates
                .FirstOrDefault(x => x.JobName == model.JobName);

            if (element == null)
            {
                element = new ExternalScheduleSyncState
                {
                    JobName = model.JobName
                };

                _context.ExternalScheduleSyncStates.Add(element);
            }

            element.LastVersionId = model.LastVersionId;
            element.LastUpdateDate = model.LastUpdateDate;
            element.LastSyncDate = model.LastSyncDate;
            element.ClassroomNumbersHash = model.ClassroomNumbersHash;

            _context.SaveChanges();

            return new ExternalScheduleSyncStateViewModel
            {
                Id = element.Id,
                JobName = element.JobName,
                LastVersionId = element.LastVersionId,
                LastUpdateDate = element.LastUpdateDate,
                LastSyncDate = element.LastSyncDate,
                ClassroomNumbersHash = element.ClassroomNumbersHash,
            };
        }
    }
}
