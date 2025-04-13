using Lentolippujärjestelmä.Models;
using Lentolippujärjestelmä.Services;

namespace Lentolippujärjestelmä.Views
{
    [QueryProperty(nameof(CurrentUser), "User")]
    public partial class UserPage : ContentPage
    {
        private User _currentUser;
        private readonly DatabaseService db;

        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
            }
        }

        public UserPage(DatabaseService databaseService)
        {
            InitializeComponent();
            db = databaseService;
            FlightDatePicker.MinimumDate = DateTime.Today;
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (_currentUser == null)
            {
                Shell.Current.DisplayAlert("Virhe", "Käyttäjän tietoja ei löytynyt", "OK");
                Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
            else
            {
                WelcomeLabel.Text = $"Tervetuloa, {_currentUser.Name}!";
            }
        }

        private async void OnSearchFlightsClicked(object sender, EventArgs e)
        {
            try
            {
                string from = DepartureEntry.Text;
                string to = DestinationEntry.Text;
                DateTime? departureDate = FlightDatePicker.Date;

                // Tietojen tarkistus
                if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
                {
                    await DisplayAlert("Virhe", "Täytä kaikki tiedot", "OK");
                    return;
                }

                // Haetaan lentoja
                await Shell.Current.GoToAsync(nameof(FlightListPage), new Dictionary<string, object>
                {
                    { "User", _currentUser },
                    { "Deparute", from },
                    { "Destination", to },
                    { "Date", departureDate?.ToString("o") }
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Virhe", $"Virhe lentojen haussa: {ex.Message}", "OK");
            }
        }

        private async void OnViewReservationsClicked(object sender, EventArgs e) 
        {
            await Shell.Current.GoToAsync(nameof(ReservationsPage), new Dictionary<string, object> { { "User", _currentUser } });
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.GoToAsync(nameof(LoginPage));
        }
    }
}