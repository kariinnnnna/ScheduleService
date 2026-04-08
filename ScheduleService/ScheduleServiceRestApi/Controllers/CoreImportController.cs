using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScheduleServiceContracts.BusinessLogicContracts;

namespace ScheduleServiceRestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CoreImportController : ControllerBase
    {
        private readonly ICoreImportLogic _coreImportLogic;

        public CoreImportController(ICoreImportLogic coreImportLogic)
        {
            _coreImportLogic = coreImportLogic;
        }

        [HttpPost]
        public async Task<IActionResult> ImportGroups()
        {
            await _coreImportLogic.ImportGroupsAsync();
            return Ok("Синхронизация групп завершена");
        }

        [HttpPost]
        public async Task<IActionResult> ImportTeachers()
        {
            await _coreImportLogic.ImportTeachersAsync();
            return Ok("Синхронизация преподавателей завершена");
        }

        [HttpPost]
        public async Task<IActionResult> ImportAll()
        {
            await _coreImportLogic.ImportAllAsync();
            return Ok("Синхронизация групп и преподавателей завершена");
        }
    }
}
