namespace API_LuegoPago.Services.Interfaces
{
    public interface IAppointmentService
    {
        AvailableSpacesResult CalculateAvailableSpaces(string day);
    }
}
