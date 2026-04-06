using ScheduleServiceDataModels.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceDataModels.Models
{
    public interface IScheduleItemModel : IId
    {
        ScheduleItemType Type { get; }
        DateTime Date { get; }
        string Subject { get; }
        int? LessonTimeId { get; }
        TimeSpan? StartTime { get; }
        TimeSpan? EndTime { get; }
        int? ClassroomId { get; }
        string? ClassroomNumber { get; }
        int? GroupId { get; }
        string? GroupName { get; }
        int? TeacherId { get; }
        string? TeacherName { get; }
        string? Comment { get; }
        bool IsImported { get; }
    }
}
