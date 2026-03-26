using Microsoft.AspNetCore.Mvc;
using ScheduleServiceContracts.BindingModels;
using ScheduleServiceContracts.BusinessLogicContracts;
using ScheduleServiceContracts.SearchModels;

namespace ScheduleServiceRestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherLogic _logic;

        public TeacherController(ITeacherLogic logic)
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
        public IActionResult GetFiltered([FromBody] TeacherSearchModel model)
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
        public IActionResult GetElement([FromBody] TeacherSearchModel model)
        {
            try
            {
                var result = _logic.ReadElement(model);
                return result == null ? NotFound("Преподаватель не найден") : Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] TeacherBindingModel model)
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
        public IActionResult Update([FromBody] TeacherBindingModel model)
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
        public IActionResult Delete([FromBody] TeacherBindingModel model)
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