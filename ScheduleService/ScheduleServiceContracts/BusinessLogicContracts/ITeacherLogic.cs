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
    public interface ITeacherLogic
    {
        List<TeacherViewModel>? ReadList(TeacherSearchModel? model);

        TeacherViewModel? ReadElement(TeacherSearchModel model);

        TeacherViewModel? Create(TeacherBindingModel model);

        TeacherViewModel? Update(TeacherBindingModel model);

        bool Delete(TeacherBindingModel model);
    }
}
