using ScheduleServiceContracts.BindingModels;
using ScheduleServiceContracts.SearchModels;
using ScheduleServiceContracts.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceContracts.BusinessLogicContracts
{
    public interface IGroupLogic
    {
        List<GroupViewModel>? ReadList(GroupSearchModel? model);

        GroupViewModel? ReadElement(GroupSearchModel model);

        GroupViewModel? Create(GroupBindingModel model);

        GroupViewModel? Update(GroupBindingModel model);

        bool Delete(GroupBindingModel model);
    }
}
