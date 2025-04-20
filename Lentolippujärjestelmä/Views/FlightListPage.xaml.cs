using Lentolippujärjestelmä.Models;
using Lentolippujärjestelmä.Services;

namespace Lentolippujärjestelmä.Views
{
    public static class FlightSession
    {
        public static Flight? EditingFlight { get; set; }
    }

    [QueryProperty(nameof(User), "User")]
    [QueryProperty(nameof(Departure), "Departure")]
    [QueryProperty(nameof(Destination), "Destination")]
    [QueryProperty(nameof(Date), "Date")]
    public partial class FlightListPage : ContentPage
    {
        private readonly DatabaseService db;
        private User _currentUser;
        private string _departure;
        private string _destination;
        private DateTime? _date;

        public User User
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
            }
        }

        public string Departure
        {
            get => _departure;
            set
            {
                _departure = value;
                OnPropertyChanged();
            }
        }

        public string Destination
        {
            get => _destination;
            set
            {
                _destination = value;
                OnPropertyChanged();
            }
        }

        public string Date
        {
            get => _date?.ToString("o");
            set
            {
                if (DateTime.TryParse(value, out var parsed))
                {
                    _date = parsed;
                    OnPropertyChanged(nameof(FlightDate));
                }
            }
        }

        public DateTime? FlightDate => _date;

        public FlightListPage(DatabaseService databaseService)
        {
            InitializeComponent();
            db = databaseService;
        }
        
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (_currentUser == null)
            {
                await DisplayAlert("Virhe", "Käyttäjän tietoja ei saatu", "OK");
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync(nameof(LoginPage));
            }
            await LoadFlights();
        }

        // Ladataan ja näytetään lennot
        private async Task LoadFlights()
        {
            try
            {
                var flights = await db.GetFlightsAsync(_departure, _destination, _date);
                FlightsListView.ItemsSource = flights;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Virhe", $"Lentojen latataaminen epäonnistui: {ex.Message}", "OK");
            }
        }

        private async void OnFlightSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is Flight selectedFlight)
            {
                try
                {
                    if (_currentUser.Role == 0) // Admin
                    {
                        FlightSession.EditingFlight = selectedFlight;
                        await Shell.Current.GoToAsync(nameof(AddFlightPage));
                    }
                    else // Normaali käyttäjä
                    {
                        bool confirm = await DisplayAlert("Varaa lento", $"Haluatko varata lennon {selectedFlight.FlightNumber}?", "Kyllä", "Ei");
                        if (confirm)
                        {
                            var reservation = new Reservation
                            {
                                ReservationNumber = Guid.NewGuid().ToString().Substring(0, 8),
                                UserId = _currentUser.Id,
                                FlightId = selectedFlight.Id,
                                BookingDate = DateTime.Now,
                                Flight = selectedFlight
                            };
                            await db.CreateReservationAsync(reservation);
                            await DisplayAlert("Onnistui", "Lento varattu onnistuneesti", "OK");
                            await LoadFlights();
                        }
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Virhe", $"Lennon varaaminen epäonnistui: {ex.Message}", "OK");
                }
                finally
                {
                    ((ListView)sender).SelectedItem = null;
                }
            }
        }
    }
}