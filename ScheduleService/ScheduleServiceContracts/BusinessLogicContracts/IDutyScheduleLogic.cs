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
    public interface IDutyScheduleLogic
    {
        List<DutyScheduleViewModel>? ReadList(DutyScheduleSearchModel? model);

        DutyScheduleViewModel? ReadElement(DutyScheduleSearchModel model);

        DutyScheduleViewModel? Create(DutyScheduleBindingModel model);

        DutyScheduleViewModel? Update(DutyScheduleBindingModel model);

        bool Delete(DutyScheduleBindingModel model);
    }
}
