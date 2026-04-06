using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScheduleServiceContracts.BindingModels;
using ScheduleServiceContracts.BusinessLogicContracts;
using System.Text;

namespace ScheduleServiceRestApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UniversityScheduleController : ControllerBase
    {
        private readonly IUniversityScheduleLogic _logic;

        public UniversityScheduleController(IUniversityScheduleLogic logic)
        {
            _logic = logic;
        }

        [HttpPost]
        public IActionResult ParseGroupSchedule([FromBody] UniversityScheduleParseBindingModel model)
        {
            try
            {
                var result = _logic.ParseGroupSchedule(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult ParseGroupScheduleFromHtml([FromBody] UniversityScheduleParseHtmlBindingModel model)
        {
            try
            {
                var result = _logic.ParseGroupScheduleFromHtml(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ParseGroupScheduleFromHtmlFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("HTML-файл не передан.");

                using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
                var html = await reader.ReadToEndAsync();

                var result = _logic.ParseGroupScheduleFromHtml(new UniversityScheduleParseHtmlBindingModel
                {
                    HtmlContent = html
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public IActionResult ImportGroupSchedulesFromFolder([FromBody] UniversityScheduleImportFolderBindingModel model)
        {
            try
            {
                _logic.ImportGroupSchedulesFromFolder(model);
                return Ok("Импорт завершен");
            }
            catch (Exception ex)
            {
                var messages = new List<string>();
                var current = ex;

                while (current != null)
                {
                    messages.Add(current.Message);
                    current = current.InnerException;
                }

                return BadRequest(string.Join(" --> ", messages));
            }
        }
    }
}
