using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceContracts.ViewModels
{
    public class ParsedUniversityScheduleViewModel
    {
        public string GroupName { get; set; } = string.Empty;
        public int WeekNumber { get; set; }
        public DateTime WeekStartDate { get; set; }
        public DateTime WeekEndDate { get; set; }

        public List<ParsedUniversityScheduleItemViewModel> Items { get; set; } = new();
    }
}
