using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceContracts.ViewModels
{
    public class TeacherViewModel
    {
        public int Id { get; set; }

        public int CoreSystemId { get; set; }

        public string TeacherName { get; set; } = string.Empty;
    }
}
