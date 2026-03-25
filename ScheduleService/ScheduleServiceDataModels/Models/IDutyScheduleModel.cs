using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceDataModels.Models
{
    //public interface IDutyScheduleModel : IId
    //{
    //    DateTime Date { get; }
    //    TimeSpan StartTime { get; }
    //    TimeSpan EndTime { get; }
    //    string? Place { get; }
    //    string? Comment { get; }
    //    int DutyPersonId { get; }
    //}

    public interface IDutyScheduleModel : IId
    {
        DateTime Date { get; }

        int? LessonTimeId { get; }

        TimeSpan? StartTime { get; }
        TimeSpan? EndTime { get; }

        string? Place { get; }
        string? Comment { get; }

        int DutyPersonId { get; }
    }
}
