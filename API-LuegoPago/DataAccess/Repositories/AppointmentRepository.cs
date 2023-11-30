using API_LuegoPago.Models;
using API_LuegoPago.Services.Interfaces;
using Newtonsoft.Json;

namespace API_LuegoPago.DataAccess.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {

        private readonly string jsonFilePath;

        public AppointmentRepository (string filePath)
        {
            jsonFilePath = filePath;
        }

        public List<Appointment> GetAppointmentsByDay (string day)
        {
            try
            {
                var jsonContent = File.ReadAllText(jsonFilePath);

                var appointments = JsonConvert.DeserializeObject<List<Appointment>>(jsonContent);

                var appointmentsForDay = appointments.Where(appointment => appointment.Day.ToLower() == day.ToLower()).ToList();

                return appointmentsForDay;

            } catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            
        }

    }
}
