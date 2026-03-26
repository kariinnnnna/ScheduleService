using Microsoft.EntityFrameworkCore;
using ScheduleServiceDataModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceDatabaseImplement.Models
{
    [Comment("Учебные группы")]
    public class Group : IGroupModel
    {
        public int Id { get; set; }

        public int CoreSystemId { get; set; }

        public string GroupName { get; set; } = string.Empty;

        public virtual List<ScheduleItem> ScheduleItems { get; set; } = new();
    }
}
