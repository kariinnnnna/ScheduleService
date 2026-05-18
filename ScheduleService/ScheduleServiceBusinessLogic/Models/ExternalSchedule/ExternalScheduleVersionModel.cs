using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ScheduleServiceBusinessLogic.Models.ExternalSchedule
{
    public class ExternalScheduleVersionModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("lessons")]
        public object? Lessons { get; set; }

        [JsonPropertyName("updateDate")]
        public string UpdateDate { get; set; } = string.Empty;
    }
}
