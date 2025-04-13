using Lentolippujärjestelmä.Models;
using Lentolippujärjestelmä.Services;

namespace Lentolippujärjestelmä.Views
{
    [QueryProperty(nameof(CurrentUser), "User")]
    public partial class AdminPage : ContentPage
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

        public AdminPage(DatabaseService databaseService)
        {
            InitializeComponent();
            db = databaseService;
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

        private async void OnViewAllFlightsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(FlightListPage), new Dictionary<string, object> { { "User", _currentUser } });
        }

        private async void OnAddNewFlightClicked(object sender, EventArgs e)
        {
            FlightSession.EditingFlight = null;
            await Shell.Current.GoToAsync(nameof(AddFlightPage));
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.GoToAsync(nameof(LoginPage));
        }
    }
}