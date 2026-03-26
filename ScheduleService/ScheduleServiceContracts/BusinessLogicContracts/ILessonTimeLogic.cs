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
    public interface ILessonTimeLogic
    {
        List<LessonTimeViewModel>? ReadList(LessonTimeSearchModel? model);

        LessonTimeViewModel? ReadElement(LessonTimeSearchModel model);

        LessonTimeViewModel? Create(LessonTimeBindingModel model);

        LessonTimeViewModel? Update(LessonTimeBindingModel model);

        bool Delete(LessonTimeBindingModel model);
    }
}
