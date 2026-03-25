using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceDataModels.Models
{
    public interface IDutyPersonModel : IId
    {
        string LastName { get; }
        string FirstName { get; }
        string? Position { get; }
        string? Phone { get; }
        string? Email { get; }
    }
}
