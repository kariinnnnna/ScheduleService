using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceContracts.ViewModels
{
    public class GroupViewModel
    {
        public int Id { get; set; }

        public int CoreSystemId { get; set; }

        public string GroupName { get; set; } = string.Empty;
    }
}
