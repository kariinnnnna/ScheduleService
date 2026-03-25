using ScheduleServiceDataModels.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceContracts.SearchModels
{
    public class ScheduleItemSearchModel
    {
        public int? Id { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public ScheduleItemType? Type { get; set; }

        public int? LessonTimeId { get; set; }

        public int? ClassroomId { get; set; }

        public int? GroupId { get; set; }

        public int? TeacherId { get; set; }

        public string? Subject { get; set; }
    }
}
