using Microsoft.EntityFrameworkCore;
using ScheduleServiceDataModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceDatabaseImplement.Models
{
    [Comment("Время учебных пар")]
    public class LessonTime : ILessonTimeModel
    {
        public int Id { get; set; }

        public int PairNumber { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public string? Description { get; set; }

        public virtual List<ScheduleItem> ScheduleItems { get; set; } = new();

        public virtual List<DutySchedule> DutySchedules { get; set; } = new();
    }
}
