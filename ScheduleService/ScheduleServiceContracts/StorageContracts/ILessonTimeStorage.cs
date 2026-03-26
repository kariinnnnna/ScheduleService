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
    public interface ILessonTimeStorage
    {
        List<LessonTimeViewModel> GetFullList();

        List<LessonTimeViewModel> GetFilteredList(LessonTimeSearchModel model);

        LessonTimeViewModel? GetElement(LessonTimeSearchModel model);

        LessonTimeViewModel? Insert(LessonTimeBindingModel model);

        LessonTimeViewModel? Update(LessonTimeBindingModel model);

        LessonTimeViewModel? Delete(LessonTimeBindingModel model);
    }
}
