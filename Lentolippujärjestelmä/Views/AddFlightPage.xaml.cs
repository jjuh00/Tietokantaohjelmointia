using Lentolippujärjestelmä.Services;
using Lentolippujärjestelmä.Models;
using System.Globalization;

namespace Lentolippujärjestelmä.Views
{
    public partial class AddFlightPage : ContentPage
    {
        private readonly DatabaseService db;
        private readonly Flight _flight;
        public Flight Flight => _flight;

        public List<int> HourOptions { get; set; } = Enumerable.Range(0, 24).ToList();
        public List<int> MinuteOptions { get; set; } = Enumerable.Range(0, 60).Where(m => m % 5 == 0).ToList();

        public AddFlightPage(DatabaseService databaseService)
            : this(databaseService, FlightSession.EditingFlight)
        {
            FlightSession.EditingFlight = null;
        }

        public AddFlightPage(DatabaseService databaseService, Flight flight = null)
        {
            InitializeComponent();
            db = databaseService;
            _flight = flight ?? new Flight();

            DepartureDatePicker.MinimumDate = DateTime.Today;

            HoursPicker.ItemsSource = HourOptions;
            MinutesPicker.ItemsSource = MinuteOptions;

            HoursPicker.SelectedItem = 0;
            MinutesPicker.SelectedItem = 0;

            BindingContext = this;

            if (flight != null && !string.IsNullOrEmpty(flight.Id))
            {
                // Muokataan olemassaolevaa lentoa
                _flight.IsExistingFlight = true;
                PopulateForm();
                ConfigureButtonsForEdit();
            }
            else
            {
                ConfigureButtonsForAdd();
            }
        }

        private void PopulateForm()
        {
            // Täytetään kentät
            FlightNumberEntry.Text = _flight.FlightNumber;
            DepartureAirportEntry.Text = _flight.DepartureAirport;
            DestinationAirportEntry.Text = _flight.DestinationAirport;
            DepartureDatePicker.Date = _flight.DepartureTime.Date;
            DepartureTimePicker.Time = _flight.DepartureTime.TimeOfDay;
            PriceEntry.Text = _flight.Price.ToString();
            CapacityEntry.Text = _flight.Capacity.ToString();

            // Lennon keston pickerit
            int hours = (int)_flight.Duration;
            int minutes = (int)((_flight.Duration % 1) * 60);
            int closestMinuteOption = MinuteOptions.OrderBy(m => Math.Abs(m - minutes)).First();

            HoursPicker.SelectedItem = hours;
            MinutesPicker.SelectedItem = closestMinuteOption;
        }

        private void ConfigureButtonsForEdit()
        {
            // Asetetaan nappien näkyvyys olemassaolevan lennon muokkausta varten
            AddFlightBtn.IsVisible = false;
            EditFlightBtn.IsVisible = true;
            DeleteFlightBtn.IsVisible = true;
        }

        private void ConfigureButtonsForAdd()
        {
            // Asetetaan nappien näkyvyys, kun lisätään uutta lentoa
            AddFlightBtn.IsVisible = true;
            EditFlightBtn.IsVisible = false;
            DeleteFlightBtn.IsVisible = false;
        }

        private async void OnAddFlightClicked(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateFlightData())
                    return;

                UpdateFlightFromForm();
                await db.InsertFlightAsync(_flight);
                await DisplayAlert("Onnistui", "Lento lisätty onnistuneesti", "OK");
                await Navigation.PopAsync();
            }
            catch (FormatException)
            {
                await DisplayAlert("Virhe", "Tarkista numeeriset kentät", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Virhe", $"Virhe lennon käsittelyssä: {ex.Message}", "OK");
            }
        }

        private async void OnUpdateFlightClicked(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateFlightData())
                    return;

                UpdateFlightFromForm();

                int reservedSeats = await db.GetReservationsForFlightAsync(_flight.Id);

                if (_flight.Capacity < reservedSeats)
                {
                    await DisplayAlert("Virhe", $"Et voi asettaa lennon kapasiteettia pienemmäksi kuin {reservedSeats}", "OK");
                    return;
                }
                
                // Päivitetään vapaat paikat uuden kapasiteetin perusteella
                _flight.AvailableSeats = _flight.Capacity - reservedSeats;

                await db.UpdateFlightAsync(_flight);
                await DisplayAlert("Onnistui", "Lento päivitetty onnistuneesti", "OK");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Virhe", $"Lennon päivittäminen epäonnistui: {ex.Message}", "OK");
            }
        }

        private async void OnDeleteFlightClicked(object sender, EventArgs e)
        {
            try
            {
                bool confirm = await DisplayAlert("Vahvista", "Haluatko varmasti poistaa tämän lennon?", "Kyllä", "Ei");
                if (!confirm)
                    return;

                // Poistetaan lento ja siihen liittyvät varaukset
                await db.DeleteFlightAsync(_flight.Id);
                await DisplayAlert("Onnistui", "Lento poistettu onnistuneesti", "OK");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Virhe", $"Virhe lennon poistamisessa: {ex.Message}", "OK");
            }
        }

        private bool ValidateFlightData()
        {
            // Tarkistetaan syötteet
            if (string.IsNullOrWhiteSpace(FlightNumberEntry.Text) ||
                string.IsNullOrWhiteSpace(DepartureAirportEntry.Text) ||
                string.IsNullOrWhiteSpace(DestinationAirportEntry.Text) ||
                !decimal.TryParse(PriceEntry.Text, out decimal price) || price <= 0 ||
                !int.TryParse(CapacityEntry.Text, out int capacity) || capacity <= 0 ||
                HoursPicker.SelectedItem == null ||
                MinutesPicker.SelectedItem == null)
            {
                DisplayAlert("Virhe", "Täytä kaikki kentät oikein", "OK").Wait();
                return false;
            }

            int hours = (int)HoursPicker.SelectedItem;
            int minutes = (int)MinutesPicker.SelectedItem;
            if (hours == 0 && minutes == 0)
            {
                DisplayAlert("Virhe", "Lennot kesto ei voi olla 0 minuuttia", "OK");
                return false;
            }

            return true;
        }

        private void UpdateFlightFromForm()
        {
            // Haetaan kenttien arvot
            _flight.FlightNumber = FlightNumberEntry.Text;
            _flight.DepartureAirport = DepartureAirportEntry.Text;
            _flight.DestinationAirport = DestinationAirportEntry.Text;
            var culture = CultureInfo.CurrentCulture;
            _flight.Price = decimal.Parse(PriceEntry.Text, culture);
            _flight.Capacity = int.Parse(CapacityEntry.Text);

            if (!_flight.IsExistingFlight)
            {
                _flight.AvailableSeats = _flight.Capacity;
            }

            var departureDate = DepartureDatePicker.Date;
            var departureTime = DepartureTimePicker.Time;
            _flight.DepartureTime = new DateTime(
                departureDate.Year,
                departureDate.Month,
                departureDate.Day,
                departureTime.Hours,
                departureTime.Minutes,
                0
            );

            int hours = (int)HoursPicker.SelectedItem;
            int minutes = (int)MinutesPicker.SelectedItem;
            _flight.Duration = hours + (minutes / 60.0);
        }
    }
}