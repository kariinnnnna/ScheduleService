using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceContracts.SearchModels
{
    public class DutyPersonSearchModel
    {
        public int? Id { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Position { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
}
