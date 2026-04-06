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
    public interface IScheduleItemStorage
    {
        List<ScheduleItemViewModel> GetFullList();

        List<ScheduleItemViewModel> GetFilteredList(ScheduleItemSearchModel model);

        ScheduleItemViewModel? GetElement(ScheduleItemSearchModel model);

        ScheduleItemViewModel? Insert(ScheduleItemBindingModel model);

        ScheduleItemViewModel? Update(ScheduleItemBindingModel model);

        ScheduleItemViewModel? Delete(ScheduleItemBindingModel model);

        void DeleteImported();
    }
}
