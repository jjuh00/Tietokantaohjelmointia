using Npgsql;
using Lentolippujärjestelmä.Models;

namespace Lentolippujärjestelmä.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString = "Host=localhost;Port=5432;Database=flight_system;Username=kayttaja;Password=salasana";

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Lisätään uusi lento tietokantaan
        public async Task InsertFlightAsync(Flight flight)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                var cmd = new NpgsqlCommand(
                    "INSERT INTO flights (flight_number, departure_airport, destination_airport, departure_time, duration, price, capacity, seats_available) " +
                    "VALUES (@flight_number, @dep_airport, @dest_airport, @time, @duration, @price, @capacity, @seats_available) RETURNING id",
                    conn);
                cmd.Parameters.AddWithValue("flight_number", flight.FlightNumber);
                cmd.Parameters.AddWithValue("dep_airport", flight.DepartureAirport);
                cmd.Parameters.AddWithValue("dest_airport", flight.DestinationAirport);
                cmd.Parameters.AddWithValue("time", flight.DepartureTime);
                cmd.Parameters.AddWithValue("duration", flight.Duration);
                cmd.Parameters.AddWithValue("price", flight.Price);
                cmd.Parameters.AddWithValue("capacity", flight.Capacity);
                cmd.Parameters.AddWithValue("seats_available", flight.AvailableSeats);
                flight.Id = (await cmd.ExecuteScalarAsync()).ToString();
                await conn.CloseAsync();
            }
            catch (NpgsqlException ex)
            {
                throw new Exception($"Virhe lennon lisäämisessä tietokantaan: {ex.Message}");
            }
        }

        // Päivitetään lennot tiedot
        public async Task UpdateFlightAsync(Flight flight)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                var cmd = new NpgsqlCommand(
                    "UPDATE flights SET flight_number = @flight_number, departure_airport = @dep_airport, destination_airport = @dest_airport, " +
                    "departure_time = @time, duration = @duration, price = @price, capacity = @capacity, seats_available = @seats_available " +
                    "WHERE id = @id",
                    conn);
                cmd.Parameters.AddWithValue("id", int.Parse(flight.Id));
                cmd.Parameters.AddWithValue("flight_number", flight.FlightNumber);
                cmd.Parameters.AddWithValue("dep_airport", flight.DepartureAirport);
                cmd.Parameters.AddWithValue("dest_airport", flight.DestinationAirport);
                cmd.Parameters.AddWithValue("time", flight.DepartureTime);
                cmd.Parameters.AddWithValue("duration", flight.Duration);
                cmd.Parameters.AddWithValue("price", flight.Price);
                cmd.Parameters.AddWithValue("capacity", flight.Capacity);
                cmd.Parameters.AddWithValue("seats_available", flight.AvailableSeats);
                await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }
            catch (NpgsqlException ex)
            {
                throw new Exception($"Virhe lennon päivittämisessä tietokantaan: {ex.Message}");
            }
        }

        // Poistetaan lento tietokannasta
        public async Task DeleteFlightAsync(string flightId)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                // Poistetaan ensin lennon varaukset
                var deleteReservationsCmd = new NpgsqlCommand("DELETE FROM reservations WHERE flight_id = @id", conn);
                deleteReservationsCmd.Parameters.AddWithValue("id", int.Parse(flightId));
                await deleteReservationsCmd.ExecuteNonQueryAsync();

                // Ja sitten lento
                var deleteFlightCmd = new NpgsqlCommand("DELETE FROM flights WHERE id = @id", conn);
                deleteFlightCmd.Parameters.AddWithValue("id", int.Parse(flightId));
                await deleteFlightCmd.ExecuteNonQueryAsync();

                await conn.CloseAsync();
            }
            catch (NpgsqlException ex)
            {
                throw new Exception($"Virhe lennon poistamisessa tietokannasta: {ex.Message}");
            }
        }

        // Haetaan käyttäjä (sähköpostilla)
        public async Task<User> GetUserByEmailAsync(string email)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                var cmd = new NpgsqlCommand("SELECT id, name, email, password, role FROM users WHERE email = @email", conn);
                cmd.Parameters.AddWithValue("email", email);
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new User
                    {
                        Id = reader.GetInt32(0).ToString(),
                        Name = reader.GetString(1),
                        Email = reader.GetString(2),
                        PasswordHash = reader.GetString(3),
                        Role = reader.GetInt16(4)
                    };
                }
            }
            catch (NpgsqlException ex)
            {
                throw new Exception($"Virhe käyttäjän hakemisessa tietokannasta: {ex.Message}");
            }

            return null;
        }

        // Tarkistetaan, onko sähköposti jo käytössä
        public async Task<bool> EmailExistsAsync(string email)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM users WHERE email = @email", conn);
                cmd.Parameters.AddWithValue("email", email);
                var count = (long)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch (NpgsqlException ex)
            {
                throw new Exception($"Virhe sähköpostin hakemisessa tietokannasta: {ex.Message}");
            }
        }

        // Lisätään uusi käyttäjä tietokantaan
        public async Task InsertUserAsync(User user)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                var cmd = new NpgsqlCommand(
                    "INSERT INTO users (name, email, password, role) VALUES (@name, @email, @password, @role) RETURNING id",
                    conn);
                cmd.Parameters.AddWithValue("name", user.Name);
                cmd.Parameters.AddWithValue("email", user.Email);
                cmd.Parameters.AddWithValue("password", user.PasswordHash);
                cmd.Parameters.AddWithValue("role", user.Role);
                user.Id = (await cmd.ExecuteScalarAsync()).ToString();
                await conn.CloseAsync();
            }
            catch (NpgsqlException ex)
            {
                throw new Exception($"Virhe käyttäjän lisäämisesäs tietokantaan: {ex.Message}");
            }
        }


        // Haetaan hakua vastaavat lennot tietokannasta
        public async Task<List<Flight>> GetFlightsAsync(string departure = null, string destination = null, DateTime? date = null)
        {
            var flights = new List<Flight>();

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                var query = "SELECT id, flight_number, departure_airport, destination_airport, departure_time, duration, price, capacity, seats_available " +
                            "FROM flights WHERE 1=1";
                if (!string.IsNullOrWhiteSpace(departure))
                {
                    query += " AND departure_airport ILIKE @departure";
                }
                if (!string.IsNullOrWhiteSpace(destination))
                {
                    query += " AND destination_airport ILIKE @destination";
                }
                if (date.HasValue)
                {
                    query += " AND DATE(departure_time) = @date";
                }

                var cmd = new NpgsqlCommand(query, conn);
                if (!string.IsNullOrWhiteSpace(departure))
                {
                    cmd.Parameters.AddWithValue("departure", $"{departure}%");
                }
                if (!string.IsNullOrWhiteSpace(destination))
                {
                    cmd.Parameters.AddWithValue("destination", $"{destination}%");
                }
                if (date.HasValue)
                {
                    cmd.Parameters.AddWithValue("date", date.Value.Date);
                }

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    flights.Add(new Flight
                    {
                        Id = reader.GetInt32(0).ToString(),
                        FlightNumber = reader.GetString(1),
                        DepartureAirport = reader.GetString(2),
                        DestinationAirport = reader.GetString(3),
                        DepartureTime = reader.GetDateTime(4),
                        Duration = (double)reader.GetDecimal(5),
                        Price = reader.GetDecimal(6),
                        Capacity = reader.GetInt32(7),
                        AvailableSeats = reader.GetInt32(8)
                    });
                }
            }
            catch (NpgsqlException ex)
            {
                throw new Exception($"Virhe lentojen hakemisessa: {ex.Message}");
            }

            return flights;
        }

        // Haetaan käyttäjän varaukset tietokannasta
        public async Task<List<Reservation>> GetReservationsAsync(string userId)
        {
            var reservations = new List<Reservation>();

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                var cmd = new NpgsqlCommand(
                    "SELECT r.id, r.reservation_number, r.user_id, r.flight_id, r.booking_date, " +
                    "f.id, f.flight_number, f.departure_airport, f.destination_airport, f.departure_time, f.duration, f.price, f.capacity, f.seats_available " +
                    "FROM reservations r JOIN flights f ON r.flight_id = f.id WHERE r.user_id = @user_id",
                    conn);
                cmd.Parameters.AddWithValue("user_id", int.Parse(userId));
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var flight = new Flight
                    {
                        Id = reader.GetInt32(5).ToString(),
                        FlightNumber = reader.GetString(6),
                        DepartureAirport = reader.GetString(7),
                        DestinationAirport = reader.GetString(8),
                        DepartureTime = reader.GetDateTime(9),
                        Duration = (double)reader.GetDecimal(10),
                        Price = reader.GetDecimal(11),
                        Capacity = reader.GetInt32(12),
                        AvailableSeats = reader.GetInt32(13)
                    };
                    reservations.Add(new Reservation
                    {
                        Id = reader.GetInt32(0).ToString(),
                        ReservationNumber = reader.GetString(1),
                        UserId = reader.GetInt32(2).ToString(),
                        FlightId = reader.GetInt32(3).ToString(),
                        BookingDate = reader.GetDateTime(4),
                        Flight = flight
                    });
                }
            }
            catch (NpgsqlException ex)
            {
                throw new Exception($"Virhe varausten haussa: {ex.Message}");
            }

            return reservations;
        }

        public async Task<Reservation> CreateReservationAsync(Reservation reservation)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            using var transaction = await conn.BeginTransactionAsync();
            try
            {
                // Päivitetään saatavilla olevat paikat
                var updateCmd = new NpgsqlCommand(
                    "UPDATE flights SET seats_available = seats_available - 1 WHERE id = @flight_id AND seats_available > 0 RETURNING seats_available",
                    conn);
                updateCmd.Parameters.AddWithValue("flight_id", int.Parse(reservation.FlightId));
                var seats = await updateCmd.ExecuteScalarAsync();
                if (seats == null)
                {
                    throw new Exception("Ei vapaita paikkoja");
                }

                // Lisätään varaus
                var insertCmd = new NpgsqlCommand(
                    "INSERT INTO reservations (reservation_number, user_id, flight_id, booking_date)" +
                    "VALUES (@res_number, @user_id, @flight_id, @booking_date) RETURNING id",
                    conn);
                insertCmd.Parameters.AddWithValue("res_number", reservation.ReservationNumber);
                insertCmd.Parameters.AddWithValue("user_id", int.Parse(reservation.UserId));
                insertCmd.Parameters.AddWithValue("flight_id", int.Parse(reservation.FlightId));
                insertCmd.Parameters.AddWithValue("booking_date", reservation.BookingDate);
                reservation.Id = (await insertCmd.ExecuteScalarAsync()).ToString();

                await transaction.CommitAsync();
                return reservation;
            }
            catch (NpgsqlException ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Virhe varauksen lisäämisessä tietokantaan: {ex.Message}");
            }
        }

        // Haetaan lennon varaukset
        public async Task<int> GetReservationsForFlightAsync(string flightId)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM reservations WHERE flight_id = @flight_id", conn);
                cmd.Parameters.AddWithValue("flight_id", int.Parse(flightId));
                var count = (long)(await cmd.ExecuteScalarAsync());
                return (int)count;
            }
            catch (NpgsqlException ex)
            {
                throw new Exception($"Virhe varausten määrän haussa: {ex.Message}");
            }
        }
    }
}
