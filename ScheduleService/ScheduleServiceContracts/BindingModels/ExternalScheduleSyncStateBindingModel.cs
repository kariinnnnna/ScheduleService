using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceContracts.BindingModels
{
    public class ExternalScheduleSyncStateBindingModel
    {
        public int Id { get; set; }

        public string JobName { get; set; } = string.Empty;

        public int LastVersionId { get; set; }

        public DateTime LastUpdateDate { get; set; }

        public DateTime LastSyncDate { get; set; }
        public string ClassroomNumbersHash { get; set; } = string.Empty;
    }
}
