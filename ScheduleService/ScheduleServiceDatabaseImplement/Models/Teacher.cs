using Microsoft.EntityFrameworkCore;
using ScheduleServiceDataModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceDatabaseImplement.Models
{
    [Comment("Преподаватели")]
    public class Teacher : ITeacherModel
    {
        public int Id { get; set; }

        public int CoreSystemId { get; set; }

        public string TeacherName { get; set; } = string.Empty;

        public virtual List<ScheduleItem> ScheduleItems { get; set; } = new();
    }
}
