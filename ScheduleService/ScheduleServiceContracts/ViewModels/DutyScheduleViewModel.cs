using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceContracts.ViewModels
{
    public class DutyScheduleViewModel
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int? LessonTimeId { get; set; }

        public int? PairNumber { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public string? Place { get; set; }

        public string? Comment { get; set; }

        public int DutyPersonId { get; set; }

        public string DutyPersonName { get; set; } = string.Empty;
    }
}
