using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceContracts.BindingModels
{
    public class UniversityScheduleParseBindingModel
    {
        public string Url { get; set; } = string.Empty;
        public string CookieHeader { get; set; } = string.Empty;
    }
}
