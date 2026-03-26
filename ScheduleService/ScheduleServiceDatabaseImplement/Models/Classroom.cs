using Microsoft.EntityFrameworkCore;
using ScheduleServiceDataModels.Enums;
using ScheduleServiceDataModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceDatabaseImplement.Models
{
    [Comment("Аудитории")]
    public class Classroom : IClassroomModel
    {
        public int Id { get; set; }

        public int CoreSystemId { get; set; }

        public string Number { get; set; } = string.Empty;

        public ClassroomType Type { get; set; }

        public int Capacity { get; set; }

        public bool NotUseInSchedule { get; set; }

        public virtual List<ScheduleItem> ScheduleItems { get; set; } = new();
    }
}
