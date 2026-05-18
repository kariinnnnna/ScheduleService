using Microsoft.AspNetCore.Mvc;
using ScheduleServiceContracts.BindingModels;
using ScheduleServiceContracts.BusinessLogicContracts;

namespace ScheduleServiceRestApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ExternalScheduleImportController : ControllerBase
    {
        private readonly IExternalScheduleImportLogic _externalScheduleImportLogic;

        public ExternalScheduleImportController(IExternalScheduleImportLogic externalScheduleImportLogic)
        {
            _externalScheduleImportLogic = externalScheduleImportLogic;
        }

        [HttpPost]
        public async Task<IActionResult> Import([FromBody] ExternalScheduleImportBindingModel model)
        {
            try
            {
                var result = await _externalScheduleImportLogic.ImportAsync(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}