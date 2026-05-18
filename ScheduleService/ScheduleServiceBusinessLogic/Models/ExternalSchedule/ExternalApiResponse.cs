using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ScheduleServiceBusinessLogic.Models.ExternalSchedule
{
    public class ExternalApiResponse<T>
    {
        [JsonPropertyName("response")]
        public T? Response { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; } = string.Empty;
    }
}
