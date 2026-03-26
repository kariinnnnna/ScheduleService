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
    public interface IClassroomStorage
    {
        List<ClassroomViewModel> GetFullList();

        List<ClassroomViewModel> GetFilteredList(ClassroomSearchModel model);

        ClassroomViewModel? GetElement(ClassroomSearchModel model);

        ClassroomViewModel? Insert(ClassroomBindingModel model);

        ClassroomViewModel? Update(ClassroomBindingModel model);

        ClassroomViewModel? Delete(ClassroomBindingModel model);
    }
}
