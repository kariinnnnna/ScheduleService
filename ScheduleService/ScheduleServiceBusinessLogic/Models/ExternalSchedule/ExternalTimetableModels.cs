using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ScheduleServiceBusinessLogic.Models.ExternalSchedule
{
    public class ExternalTimetableResponseBody
    {
        [JsonPropertyName("weeks")]
        public Dictionary<string, ExternalTimetableWeek> Weeks { get; set; } = new();
    }

    public class ExternalTimetableWeek
    {
        [JsonPropertyName("days")]
        public List<ExternalTimetableDay> Days { get; set; } = new();
    }

    public class ExternalTimetableDay
    {
        [JsonPropertyName("day")]
        public int Day { get; set; }

        [JsonPropertyName("lessons")]
        public List<List<ExternalTimetableLesson>> Lessons { get; set; } = new();
    }

    public class ExternalTimetableLesson
    {
        [JsonPropertyName("group")]
        public string Group { get; set; } = string.Empty;

        [JsonPropertyName("nameOfLesson")]
        public string NameOfLesson { get; set; } = string.Empty;

        [JsonPropertyName("teacher")]
        public string Teacher { get; set; } = string.Empty;

        [JsonPropertyName("room")]
        public string Room { get; set; } = string.Empty;
    }

    public class ExternalScheduleLessonModel
    {
        public int StudyWeek { get; set; }
        public int Day { get; set; }
        public int PairNumber { get; set; }

        public string GroupName { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public string ClassroomNumber { get; set; } = string.Empty;
        public string LessonName { get; set; } = string.Empty;
    }
}
