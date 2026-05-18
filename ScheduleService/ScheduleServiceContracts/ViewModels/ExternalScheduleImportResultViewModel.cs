using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceContracts.ViewModels
{
    public class ExternalScheduleImportResultViewModel
    {
        public int TotalGroupsCount { get; set; }
        public int ProcessedGroupsCount { get; set; }

        public int ReceivedLessonsCount { get; set; }
        public int FilteredByClassroomCount { get; set; }
        public int GroupedLessonsCount { get; set; }

        public int CreatedCount { get; set; }
        public int SkippedCount { get; set; }
        public int ErrorCount { get; set; }

        public List<string> Errors { get; set; } = new();
        public bool SkippedByVersion { get; set; }
        public int? CurrentVersionId { get; set; }
        public int? PreviousVersionId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
