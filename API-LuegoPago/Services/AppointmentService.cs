using API_LuegoPago.Models;
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

        public AvailableSpacesResult CalculateAvailableSpaces(string day)
        {
            var result = new AvailableSpacesResult();
            try
            {
                var scheduledAppointments = GetFilteredAppointments(day);

                int totalMinutesAvailable = CalculateTotalMinutesAvailable();

                SubtractTimeBeforeFirstAppointment(scheduledAppointments, ref totalMinutesAvailable);
                SubtractDurationOfAppointments(scheduledAppointments, ref totalMinutesAvailable);
                SubtractTimeBetweenAppointments(scheduledAppointments, ref totalMinutesAvailable);
                SubtractTimeAfterLastAppointment(scheduledAppointments, ref totalMinutesAvailable);

                int totalSpacesAvailable = totalMinutesAvailable / 30;

                result.Messages.Add($"El total de espacios disponibles para el día {day} es de: {totalSpacesAvailable} ");
                result.TotalMinutesAvailable = Math.Max(totalMinutesAvailable, 0);

                return result;
            }
            catch (Exception ex)
            {
                result.Messages.Add($"Error al calcular espacios disponibles: {ex.Message}");
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

        private List<Appointment> GetFilteredAppointments(string day)
        {
            var scheduledAppointments = _appointmentRepository.GetAppointmentsByDay(day);
            Console.WriteLine($"Citas programadas para el día {day}: {scheduledAppointments.Count}");

            return scheduledAppointments
                .Where(appointment =>
                    WorkingHours(appointment))
                .OrderBy(appointment => DateTime.ParseExact(appointment.Hour, "HH:mm", CultureInfo.InvariantCulture))
                .ToList();
        }

        private void SubtractTimeBeforeFirstAppointment(List<Appointment> appointments, ref int totalMinutesAvailable)
        {
            var firstAppointment = appointments.FirstOrDefault();
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
        }

        private void SubtractDurationOfAppointments(List<Appointment> appointments, ref int totalMinutesAvailable)
        {
            totalMinutesAvailable -= appointments.Sum(appointment => appointment.Duration);
        }

        private void SubtractTimeBetweenAppointments(List<Appointment> appointments, ref int totalMinutesAvailable)
        {
            for (int i = 0; i < appointments.Count - 1; i++)
            {
                var currentAppointment = appointments[i];
                var nextAppointment = appointments[i + 1];

                var currentEndTime = DateTime.ParseExact(currentAppointment.Hour, "HH:mm", CultureInfo.InvariantCulture).AddMinutes(currentAppointment.Duration).TimeOfDay;
                var nextStartTime = DateTime.ParseExact(nextAppointment.Hour, "HH:mm", CultureInfo.InvariantCulture).TimeOfDay;

                int spaceBetweenAppointments = (int)(nextStartTime - currentEndTime).TotalMinutes;

                if (spaceBetweenAppointments < 30)
                {
                    totalMinutesAvailable -= spaceBetweenAppointments;
                }
            }
        }

        private void SubtractTimeAfterLastAppointment(List<Appointment> appointments, ref int totalMinutesAvailable)
        {
            var lastAppointment = appointments.LastOrDefault();
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
        }

        private bool WorkingHours(Appointment appointment)
        {
            var startTime = DateTime.ParseExact(appointment.Hour, "HH:mm", CultureInfo.InvariantCulture).TimeOfDay;
            var endTime = startTime.Add(TimeSpan.FromMinutes(appointment.Duration));
            return startTime >= TimeSpan.Parse("09:00") && endTime <= TimeSpan.Parse("17:00");
        }
    }
}
