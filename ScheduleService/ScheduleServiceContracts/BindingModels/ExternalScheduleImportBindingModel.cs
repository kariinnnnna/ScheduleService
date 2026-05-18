using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceContracts.BindingModels
{
    public class ExternalScheduleImportBindingModel
    {
        public List<string> ClassroomNumbers { get; set; } = new();

        public DateTime? BaseDate { get; set; }

        public bool ForceImport { get; set; }
    }
}
