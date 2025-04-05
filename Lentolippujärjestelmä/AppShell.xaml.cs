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
        }
    }
}
