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
    public interface ITeacherStorage
    {
        List<TeacherViewModel> GetFullList();

        List<TeacherViewModel> GetFilteredList(TeacherSearchModel model);

        TeacherViewModel? GetElement(TeacherSearchModel model);

        TeacherViewModel? Insert(TeacherBindingModel model);

        TeacherViewModel? Update(TeacherBindingModel model);

        TeacherViewModel? Delete(TeacherBindingModel model);
    }
}
