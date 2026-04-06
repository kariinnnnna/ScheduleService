using Microsoft.AspNetCore.Mvc;
using ScheduleServiceContracts.BindingModels;
using ScheduleServiceContracts.BusinessLogicContracts;
using ScheduleServiceContracts.SearchModels;

namespace ScheduleServiceRestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DutyPersonController : ControllerBase
    {
        private readonly IDutyPersonLogic _logic;

        public DutyPersonController(IDutyPersonLogic logic)
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
        public IActionResult GetFiltered([FromBody] DutyPersonSearchModel model)
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
        public IActionResult GetElement([FromBody] DutyPersonSearchModel model)
        {
            try
            {
                var result = _logic.ReadElement(model);
                return result == null ? NotFound("Дежурный не найден") : Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] DutyPersonBindingModel model)
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
        public IActionResult Update([FromBody] DutyPersonBindingModel model)
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
        public IActionResult Delete([FromBody] int id)
        {
            try
            {
                return Ok(_logic.Delete(new DutyPersonBindingModel { Id = id }));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}