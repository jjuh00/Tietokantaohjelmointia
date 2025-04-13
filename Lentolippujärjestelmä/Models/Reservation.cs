namespace Lentolippujärjestelmä.Models
{
    public class Reservation
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ReservationNumber { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string FlightId { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; } = DateTime.Now;
        public Flight Flight { get; set; } = null!;
    }
}
