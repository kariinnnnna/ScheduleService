using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceContracts.SearchModels
{
    public class ClassroomSearchModel
    {
        public int? Id { get; set; }

        public int? CoreSystemId { get; set; }

        public string? Number { get; set; }

        public bool? NotUseInSchedule { get; set; }
    }
}
