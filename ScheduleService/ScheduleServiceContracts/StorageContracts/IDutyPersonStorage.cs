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
    public interface IDutyPersonStorage
    {
        List<DutyPersonViewModel> GetFullList();

        List<DutyPersonViewModel> GetFilteredList(DutyPersonSearchModel model);

        DutyPersonViewModel? GetElement(DutyPersonSearchModel model);

        DutyPersonViewModel? Insert(DutyPersonBindingModel model);

        DutyPersonViewModel? Update(DutyPersonBindingModel model);

        DutyPersonViewModel? Delete(DutyPersonBindingModel model);
    }
}
