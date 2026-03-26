using ScheduleServiceContracts.BindingModels;
using ScheduleServiceContracts.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceContracts.BusinessLogicContracts
{
    public interface IBookingLogic
    {
        bool IsClassroomAvailable(ScheduleItemBindingModel model);

        ScheduleItemViewModel? BookClassroom(ScheduleItemBindingModel model);
    }
}
