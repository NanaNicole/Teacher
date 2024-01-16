using Colegio.Models;
using Colegio.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Colegio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeacherController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<TeacherController> _logger;
        private ColegioContext _dbcontext;
        private readonly ITeacher _teacher;

        public TeacherController(ILogger<TeacherController> logger, ColegioContext dbcontext, ITeacher teacher)
        {
            _logger = logger;
            _dbcontext = dbcontext;
            _teacher = teacher;
        }

        [HttpGet]
        [Route("GetTeachers")]
        public IActionResult GetTeachers()
        {

            return Ok(_teacher.GetTeachers());
        }

        [HttpGet]
        [Route("GetTeacherByIdeentification/{identification}")]
        public IActionResult GetTeacherByIdeentification(int identification)
        {
            return Ok(_teacher.GetTeacherByIdeentification(identification));
        }

        [HttpPost]
        [Route("CreateTeacher")]
        public IActionResult CreateTeacher(TeacherDto estudent)
        {
            return Ok(_teacher.CreateTeacher(estudent));
        }

        [HttpDelete]
        [Route("DeleteTeacher/{id}")]
        public IActionResult DeleteTeacher(Guid id)
        {
            try
            {
                if (_teacher.DeleteTeacher(id))
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("No se pudo eliminar correctamente el usuario");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error eliminando usuarios {ex.Message.ToString()}");
            }
        }


    }
}