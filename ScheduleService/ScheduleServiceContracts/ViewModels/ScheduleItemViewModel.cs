using ScheduleServiceDataModels.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceContracts.ViewModels
{
    public class ScheduleItemViewModel
    {
        public int Id { get; set; }

        public ScheduleItemType Type { get; set; }

        public string TypeName => Type.ToString();

        public DateTime Date { get; set; }

        public string Subject { get; set; } = string.Empty;

        public int? LessonTimeId { get; set; }

        public int? PairNumber { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public int? ClassroomId { get; set; }

        public string? ClassroomNumber { get; set; }

        public int? GroupId { get; set; }

        public string? GroupName { get; set; }

        public int? TeacherId { get; set; }

        public string? TeacherName { get; set; }

        public string? Comment { get; set; }
        public bool IsImported { get; set; }
    }
}
