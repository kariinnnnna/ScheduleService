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
    public interface IGroupStorage
    {
        List<GroupViewModel> GetFullList();

        List<GroupViewModel> GetFilteredList(GroupSearchModel model);

        GroupViewModel? GetElement(GroupSearchModel model);

        GroupViewModel? Insert(GroupBindingModel model);

        GroupViewModel? Update(GroupBindingModel model);

        GroupViewModel? Delete(GroupBindingModel model);
    }
}
