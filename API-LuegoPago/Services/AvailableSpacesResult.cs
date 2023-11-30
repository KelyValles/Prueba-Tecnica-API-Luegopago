namespace API_LuegoPago.Services
{
    public class AvailableSpacesResult
    {
        public int TotalMinutesAvailable { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
    }
}
