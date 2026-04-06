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
    public interface IScheduleItemLogic
    {
        List<ScheduleItemViewModel>? ReadList(ScheduleItemSearchModel? model);

        ScheduleItemViewModel? ReadElement(ScheduleItemSearchModel model);

        ScheduleItemViewModel? Create(ScheduleItemBindingModel model);

        ScheduleItemViewModel? Update(ScheduleItemBindingModel model);

        bool Delete(ScheduleItemBindingModel model);

    }
}
