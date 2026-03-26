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
    public interface IDutyPersonLogic
    {
        List<DutyPersonViewModel>? ReadList(DutyPersonSearchModel? model);

        DutyPersonViewModel? ReadElement(DutyPersonSearchModel model);

        DutyPersonViewModel? Create(DutyPersonBindingModel model);

        DutyPersonViewModel? Update(DutyPersonBindingModel model);

        bool Delete(DutyPersonBindingModel model);
    }
}
