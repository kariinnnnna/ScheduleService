using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceDataModels.Models
{
    public interface IGroupModel : IId
    {
        int CoreSystemId { get; }
        string GroupName { get; }
    }
}
