using ScheduleServiceDataModels.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceContracts.ViewModels
{
    public class ClassroomViewModel
    {
        public int Id { get; set; }

        public int CoreSystemId { get; set; }

        public string Number { get; set; } = string.Empty;

        public ClassroomType Type { get; set; }

        public string TypeName => Type.ToString();

        public int Capacity { get; set; }

        public bool NotUseInSchedule { get; set; }
    }
}
