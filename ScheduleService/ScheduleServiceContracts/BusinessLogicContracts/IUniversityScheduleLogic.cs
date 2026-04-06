using ScheduleServiceContracts.BindingModels;
using ScheduleServiceContracts.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceContracts.BusinessLogicContracts
{
    public interface IUniversityScheduleLogic
    {
        ParsedUniversityScheduleViewModel ParseGroupSchedule(UniversityScheduleParseBindingModel model);
        ParsedUniversityScheduleViewModel ParseGroupScheduleFromHtml(UniversityScheduleParseHtmlBindingModel model);
        void ImportGroupSchedulesFromFolder(UniversityScheduleImportFolderBindingModel model);

    }
}
