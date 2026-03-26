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
    public interface IDutyScheduleStorage
    {
        List<DutyScheduleViewModel> GetFullList();

        List<DutyScheduleViewModel> GetFilteredList(DutyScheduleSearchModel model);

        DutyScheduleViewModel? GetElement(DutyScheduleSearchModel model);

        DutyScheduleViewModel? Insert(DutyScheduleBindingModel model);

        DutyScheduleViewModel? Update(DutyScheduleBindingModel model);

        DutyScheduleViewModel? Delete(DutyScheduleBindingModel model);
    }
}
