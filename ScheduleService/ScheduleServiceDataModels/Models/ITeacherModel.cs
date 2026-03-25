using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceDataModels.Models
{
    public interface ITeacherModel : IId
    {
        int CoreSystemId { get; }
        string TeacherName { get; }
    }
}
