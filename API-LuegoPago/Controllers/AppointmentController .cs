using API_LuegoPago.Attributes;
using API_LuegoPago.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace API_LuegoPago.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IAppointmentRepository _appointmentRepository;

        public AppointmentController(IAppointmentService appointmentService, IAppointmentRepository appointmentRepository)
        {
            _appointmentService = appointmentService;
            _appointmentRepository = appointmentRepository;
        }



        // GET: api/<AppointmentController>
        [HttpGet("availableSpaces/{day}")]
        public IActionResult GetAvailableSpaces([ValidDay] string day)
        {
                var availableSpaces = _appointmentService.CalculateAvailableSpaces(day);
                return Ok(availableSpaces);
        }


        [HttpGet("appointments/{day}")]
        public IActionResult GetAppointmentsByDay([ValidDay] string day)
        {
                var appointments = _appointmentRepository.GetAppointmentsByDay(day);

                return Ok(appointments);
        }




    }
}
