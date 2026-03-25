using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceDataModels.Models
{
    public interface ILessonTimeModel : IId
    {
        int PairNumber { get; }
        TimeSpan StartTime { get; }
        TimeSpan EndTime { get; }
        string? Description { get; }
    }
}
