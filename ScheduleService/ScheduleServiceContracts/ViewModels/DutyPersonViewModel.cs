using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceContracts.ViewModels
{
    public class DutyPersonViewModel
    {
        public int Id { get; set; }

        public string LastName { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string FullName => $"{LastName} {FirstName}".Trim();

        public string? Position { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }
    }
}
