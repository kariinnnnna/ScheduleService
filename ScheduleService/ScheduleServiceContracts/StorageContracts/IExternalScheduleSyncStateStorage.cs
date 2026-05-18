using ScheduleServiceContracts.BindingModels;
using ScheduleServiceContracts.SearchModels;
using ScheduleServiceContracts.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceContracts.StorageContracts
{
    public interface IExternalScheduleSyncStateStorage
    {
        ExternalScheduleSyncStateViewModel? GetElement(ExternalScheduleSyncStateSearchModel model);

        ExternalScheduleSyncStateViewModel? InsertOrUpdate(ExternalScheduleSyncStateBindingModel model);
    }
}
