using Microsoft.AspNetCore.Mvc;
using ScheduleServiceContracts.BindingModels;
using ScheduleServiceContracts.BusinessLogicContracts;
using ScheduleServiceContracts.SearchModels;

namespace ScheduleServiceRestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ScheduleItemController : ControllerBase
    {
        private readonly IScheduleItemLogic _logic;

        public ScheduleItemController(IScheduleItemLogic logic)
        {
            _logic = logic;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(_logic.ReadList(null));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult GetFiltered([FromBody] ScheduleItemSearchModel model)
        {
            try
            {
                return Ok(_logic.ReadList(model));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult GetElement([FromBody] ScheduleItemSearchModel model)
        {
            try
            {
                var result = _logic.ReadElement(model);
                return result == null ? NotFound("Запись расписания не найдена") : Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] ScheduleItemBindingModel model)
        {
            try
            {
                return Ok(_logic.Create(model));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Update([FromBody] ScheduleItemBindingModel model)
        {
            try
            {
                return Ok(_logic.Update(model));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Delete([FromBody] ScheduleItemBindingModel model)
        {
            try
            {
                return Ok(_logic.Delete(model));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}