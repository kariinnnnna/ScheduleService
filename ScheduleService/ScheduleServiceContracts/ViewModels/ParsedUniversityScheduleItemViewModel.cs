using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceContracts.ViewModels
{
    public class ParsedUniversityScheduleItemViewModel
    {
        public string GroupName { get; set; } = string.Empty;
        public string DayName { get; set; } = string.Empty;
        public DateTime Date { get; set; }

        public int PairNumber { get; set; }
        public string TimeRange { get; set; } = string.Empty;

        public string LessonType { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public string ClassroomNumber { get; set; } = string.Empty;
        public string Subgroup { get; set; } = string.Empty;

        public string RawCellText { get; set; } = string.Empty;
    }
}
