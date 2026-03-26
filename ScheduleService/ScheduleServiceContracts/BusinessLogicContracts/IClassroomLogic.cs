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
    public interface IClassroomLogic
    {
        List<ClassroomViewModel>? ReadList(ClassroomSearchModel? model);

        ClassroomViewModel? ReadElement(ClassroomSearchModel model);

        ClassroomViewModel? Create(ClassroomBindingModel model);

        ClassroomViewModel? Update(ClassroomBindingModel model);

        bool Delete(ClassroomBindingModel model);
    }
}
