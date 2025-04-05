namespace Lentolippujärjestelmä.Models
{
    public class Flight
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FlightNumber { get; set; } = string.Empty;
        public string DepartureAirport {  get; set; } = string.Empty;
        public string DestinationAirport {  get; set; } = string.Empty;
        public DateTime DeparuteTime { get; set; }
        public double Duration { get; set; } // Tunteina
        public decimal Price { get; set; }
        public int Capacity { get; set; }
        public int AvailableSeats { get; set; }

        public string Route => $"{DepartureAirport} → {DestinationAirport}";
    }
}
