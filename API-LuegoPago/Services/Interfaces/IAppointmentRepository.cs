using API_LuegoPago.Models;

namespace API_LuegoPago.Services.Interfaces
{
    public interface IAppointmentRepository
    {
        List<Appointment> GetAppointmentsByDay(string day);
    }
}
