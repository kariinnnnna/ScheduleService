using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceContracts.SearchModels
{
    public class TeacherSearchModel
    {
        public int? Id { get; set; }

        public int? CoreSystemId { get; set; }

        public string? TeacherName { get; set; }
    }
}
