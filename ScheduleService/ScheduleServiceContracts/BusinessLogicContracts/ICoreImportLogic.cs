using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceContracts.BusinessLogicContracts
{
    public interface ICoreImportLogic
    {
        Task ImportGroupsAsync();
        Task ImportTeachersAsync();
        Task ImportAllAsync();
    }
}
