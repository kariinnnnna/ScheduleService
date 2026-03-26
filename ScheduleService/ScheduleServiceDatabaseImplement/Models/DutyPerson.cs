using Microsoft.EntityFrameworkCore;
using ScheduleServiceDataModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceDatabaseImplement.Models
{
    [Comment("Дежурные")]
    public class DutyPerson : IDutyPersonModel
    {
        public int Id { get; set; }

        public string LastName { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string? Position { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public virtual List<DutySchedule> DutySchedules { get; set; } = new();
    }
}
