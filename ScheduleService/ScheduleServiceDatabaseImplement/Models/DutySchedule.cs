using Microsoft.EntityFrameworkCore;
using ScheduleServiceDataModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceDatabaseImplement.Models
{
    [Comment("График дежурств")]
    public class DutySchedule : IDutyScheduleModel
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int? LessonTimeId { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public string? Place { get; set; }

        public string? Comment { get; set; }

        public int DutyPersonId { get; set; }

        public virtual DutyPerson DutyPerson { get; set; } = null!;

        public virtual LessonTime? LessonTime { get; set; }
    }
}
