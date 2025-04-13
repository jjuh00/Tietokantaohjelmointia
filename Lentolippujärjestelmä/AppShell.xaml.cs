namespace Lentolippujärjestelmä
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Reitit navigointia varten
            Routing.RegisterRoute(nameof(Views.LoginPage), typeof(Views.LoginPage));
            Routing.RegisterRoute(nameof(Views.RegistrationPage), typeof(Views.RegistrationPage));
            Routing.RegisterRoute(nameof(Views.UserPage), typeof(Views.UserPage));
            Routing.RegisterRoute(nameof(Views.AdminPage), typeof(Views.AdminPage));
            Routing.RegisterRoute(nameof(Views.FlightListPage), typeof(Views.FlightListPage));
            Routing.RegisterRoute(nameof(Views.AddFlightPage), typeof(Views.AddFlightPage));
            Routing.RegisterRoute(nameof(Views.ReservationsPage), typeof(Views.ReservationsPage));
        }

        protected override bool OnBackButtonPressed()
        {
            // Estetään navigointi kirjautumis- tai rekisteröitymissivulle takaisin-napilla
            if (Current.Navigation.NavigationStack.Count > 1)
            {
                var currentPage = Current.Navigation.NavigationStack.Last();
                if (currentPage is Views.UserPage || currentPage is Views.AdminPage)
                {
                    return true;
                }
            }
            return base.OnBackButtonPressed();
        }
    }
}
