using API_LuegoPago.Services.Interfaces;
using System.Globalization;

namespace API_LuegoPago.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public int CalculateAvailableSpaces(string day)
        {
            try
            {
                //obtengo los datos filtrados por día
                var scheduledAppointments = _appointmentRepository.GetAppointmentsByDay(day);
                Console.WriteLine($"Citas programadas para el día {day}: {scheduledAppointments.Count}");

                //la filtro de acuerdo al horario
                scheduledAppointments = scheduledAppointments
                .Where(appointment =>
                    DateTime.ParseExact(appointment.Hour, "HH:mm", CultureInfo.InvariantCulture).TimeOfDay >= TimeSpan.Parse("09:00") &&
                    DateTime.ParseExact(appointment.Hour, "HH:mm", CultureInfo.InvariantCulture).AddMinutes(appointment.Duration).TimeOfDay <= TimeSpan.Parse("17:00"))
                .ToList();

                //ordenos los datos
                scheduledAppointments.Sort((a1, a2) => DateTime.ParseExact(a1.Hour, "HH:mm", CultureInfo.InvariantCulture)
                .CompareTo(DateTime.ParseExact(a2.Hour, "HH:mm", CultureInfo.InvariantCulture)));

                //traigo el total de min durante horario
                int totalMinutesAvailable = CalculateTotalMinutesAvailable();
                Console.WriteLine($"Minutos totales disponibles antes de restar citas: {totalMinutesAvailable}");

                //filtro entre el opening y la primera cita
                var firstAppointment = scheduledAppointments.FirstOrDefault();
                if (firstAppointment != null)
                {
                    var openingTime = TimeSpan.Parse("09:00");
                    var firstAppointmentStartTime = DateTime.ParseExact(firstAppointment.Hour, "HH:mm", CultureInfo.InvariantCulture).TimeOfDay;

                    int spaceBeforeFirstAppointment = (int)(firstAppointmentStartTime - openingTime).TotalMinutes;
                    
                    if (spaceBeforeFirstAppointment < 30)
                    {
                        totalMinutesAvailable -= spaceBeforeFirstAppointment;
                    }
                }

                foreach (var currentAppointment in scheduledAppointments)
                {
                    totalMinutesAvailable -= currentAppointment.Duration;
                }


                //obtengo el espacio entre cita y cita
                for (int i = 0; i < scheduledAppointments.Count - 1; i++)
                {
                    var currentAppointment = scheduledAppointments[i];
                    var nextAppointment = scheduledAppointments[i + 1];

                    var currentEndTime = DateTime.ParseExact(currentAppointment.Hour, "HH:mm", CultureInfo.InvariantCulture).AddMinutes(currentAppointment.Duration);
                    var nextStartTime = DateTime.ParseExact(nextAppointment.Hour, "HH:mm", CultureInfo.InvariantCulture);

                    int spaceBetweenAppointments = (int)(nextStartTime - currentEndTime).TotalMinutes;

                    if (spaceBetweenAppointments < 30)
                    {
                        totalMinutesAvailable -= spaceBetweenAppointments;
                    }

                }

                

                //filtro entre la última cita y el closing
                var lastAppointment = scheduledAppointments.LastOrDefault();
                if (lastAppointment != null)
                {
                    var closingTime = TimeSpan.Parse("17:00");
                    var lastAppointmentStartTime = DateTime.ParseExact(lastAppointment.Hour, "HH:mm", CultureInfo.InvariantCulture).AddMinutes(lastAppointment.Duration).TimeOfDay;

                    int spaceLastAppointment = (int)(closingTime - lastAppointmentStartTime).TotalMinutes;

                    if (spaceLastAppointment < 30)
                    {
                        totalMinutesAvailable -= spaceLastAppointment;
                    }
                }

                Console.WriteLine($"Minutos totales disponibles después de restar citas: {totalMinutesAvailable}");

                return Math.Max(totalMinutesAvailable, 0);

            } catch (Exception ex)
            {
                Console.WriteLine($"Error al calcular espacios disponibles: {ex.Message}");
                throw;
            }
        }

        private int CalculateTotalMinutesAvailable()
        {
            DateTime openingTime = DateTime.ParseExact("09:00", "HH:mm", CultureInfo.InvariantCulture);
            DateTime closingTime = DateTime.ParseExact("17:00", "HH:mm",CultureInfo.InvariantCulture);
            TimeSpan WorkingHours = closingTime-openingTime;

            return (int)WorkingHours.TotalMinutes;
        }
    }
}
