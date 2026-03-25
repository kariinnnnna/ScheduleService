using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceContracts.SearchModels
{
    public class DutyScheduleSearchModel
    {
        public int? Id { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public int? DutyPersonId { get; set; }

        public int? LessonTimeId { get; set; }
    }
}
