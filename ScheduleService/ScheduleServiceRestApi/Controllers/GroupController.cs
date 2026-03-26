using Microsoft.AspNetCore.Mvc;
using ScheduleServiceContracts.BindingModels;
using ScheduleServiceContracts.BusinessLogicContracts;
using ScheduleServiceContracts.SearchModels;

namespace ScheduleServiceRestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupLogic _logic;

        public GroupController(IGroupLogic logic)
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
        public IActionResult GetFiltered([FromBody] GroupSearchModel model)
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
        public IActionResult GetElement([FromBody] GroupSearchModel model)
        {
            try
            {
                var result = _logic.ReadElement(model);
                return result == null ? NotFound("Группа не найдена") : Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] GroupBindingModel model)
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
        public IActionResult Update([FromBody] GroupBindingModel model)
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
        public IActionResult Delete([FromBody] GroupBindingModel model)
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