using Microsoft.EntityFrameworkCore;
using RentACar.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RentACar.View
{

    public partial class ReservationMaker : UserControl, INotifyPropertyChanged
    {

        private bool _isPricingVisible = false;
        private rentacarContext _dbContext;
        private Korisnik _currentUser;
        private DateTime _startDate;
        private DateTime _endDate;
        private int _totalDays = 0;
        private decimal _basePrice = 0;
        private decimal _optionsPrice = 0;
        private bool _isWeeklyRate = false;
        private List<DateOnly> _blackoutDates;
        private ObservableCollection<RentalOptionnViewModel> _availableOptions;
        private ObservableCollection<Vozilo> _availableVehicles;
        private ObservableCollection<ClientViewModel> _filteredClients;
        private Vozilo _selectedVehicle;
        private ClientViewModel _selectedClient;
        private ListCollectionView _clientsView;
        private ListCollectionView _vehiclesView;

        
        public delegate void ReservationCompletedEventHandler(object sender, ReservationCompletedEventArgs e);
        public event ReservationCompletedEventHandler ReservationCompleted;
        public event EventHandler ReservationCancelled;
        public event PropertyChangedEventHandler PropertyChanged;

        public Korisnik CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged(nameof(CurrentUser));
            }
        }

        public ReservationMaker()
        {
            InitializeComponent();
            _dbContext = new rentacarContext();
            
            _blackoutDates = new List<DateOnly>();
            _availableVehicles = new ObservableCollection<Vozilo>();
            _filteredClients = new ObservableCollection<ClientViewModel>();

            
            ReservationCalendar.DisplayDateStart = DateTime.Today;

            
            LoadGlobalBlackoutDates();

            
            RateTypeComboBox.SelectedIndex = 0;

            
            DateSelectionGrid.Visibility = Visibility.Visible;
            VehicleSelectionGrid.Visibility = Visibility.Collapsed;
            ClientOptionsGrid.Visibility = Visibility.Collapsed;

            LoadClients();
            LoadOptions();
        }
        public void Reset()
        {

            
            DateSelectionGrid.Visibility = Visibility.Visible;
            VehicleSelectionGrid.Visibility = Visibility.Collapsed;
            ClientOptionsGrid.Visibility = Visibility.Collapsed;
            PricingGrid.Visibility = Visibility.Collapsed;
            _isPricingVisible = false;

            
            ReservationCalendar.SelectedDates.Clear();
            StartDateTextBox.Text = string.Empty;
            EndDateTextBox.Text = string.Empty;
            TotalDaysTextBox.Text = string.Empty;
            _startDate = DateTime.MinValue;
            _endDate = DateTime.MinValue;
            _totalDays = 0;

            
            _selectedVehicle = null;
            if (_vehiclesView != null)
            {
                _vehiclesView.Filter = null;
                VehiclesListView.SelectedItem = null;
            }
            VehicleSearchTextBox.Text = string.Empty;

            
            _selectedClient = null;
            if (_clientsView != null)
            {
                _clientsView.Filter = null;
                ClientsListView.SelectedItem = null;
            }
            ClientSearchTextBox.Text = string.Empty;

            
            if (_availableOptions != null)
            {
                foreach (var option in _availableOptions)
                {
                    option.IsSelected = false;
                }
            }
            _optionsPrice = 0;

            
            _basePrice = 0;
            _isWeeklyRate = false;
            RateTypeComboBox.SelectedIndex = 0;
            UpdateTotalPrice();

            
            AdditionalNotesTextBox.Text = string.Empty;

            
            LoadClients();
            LoadOptions();

            
            ReservationCalendar.DisplayDateStart = DateTime.Today;
            LoadGlobalBlackoutDates();
        }

        private void LoadGlobalBlackoutDates()
        {
            
            ReservationCalendar.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, DateTime.Today.AddDays(-1)));
        }

        private void LoadClients()
        {
            try
            {
                var clients = _dbContext.Klijents.ToList();
                _filteredClients = new ObservableCollection<ClientViewModel>(
                    clients.Select(c => new ClientViewModel
                    {
                        IdKlijent = c.IdKlijent,
                        Ime = c.Ime,
                        Prezime = c.Prezime,
                        FullName = $"{c.Ime} {c.Prezime}",
                        Email = c.Email,
                        Telefon = c.BrojTelefona
                    })
                );

                _clientsView = new ListCollectionView(_filteredClients);
                ClientsListView.ItemsSource = _clientsView;
            }
            catch (Exception ex)
            {
                ShowMessage(FindResource("Error").ToString(),
                    $"{FindResource("ErrorLoadingClients")}: {ex.Message}", MessageType.Error);
            }
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                LoadClients();  // Ucitavanje klijenata (dodavanje iz managementa)
            }

        }

        private void LoadOptions()
        {
            try
            {
                var options = _dbContext.Opcijas.ToList();
                _availableOptions = new ObservableCollection<RentalOptionnViewModel>(
                    options.Select(o => new RentalOptionnViewModel(o))
                );

                OptionsItemsControl.ItemsSource = _availableOptions;
            }
            catch (Exception ex)
            {
                ShowMessage(FindResource("Error").ToString(),
                    $"{FindResource("ErrorLoadingOptions")}: {ex.Message}", MessageType.Error);
            }
        }

        private void LoadAvailableVehicles()
        {
            try
            {
                
                var startDateOnly = DateOnly.FromDateTime(_startDate);
                var endDateOnly = DateOnly.FromDateTime(_endDate);

                
                var allVehicles = _dbContext.Vozilos
                    .Include(v => v.KategorijaVozilaIdKategorijaVozilaNavigation)
                    .ToList();

                
                var overlappingRentals = _dbContext.Iznajmljivanjes
                    .Where(r =>
                        (r.DatumIznajmljivanja <= endDateOnly && r.DatumVracanja >= startDateOnly) ||
                        (r.DatumIznajmljivanja <= endDateOnly && r.DatumVracanja == null))
                    .ToList();

                
                var unavailableVehicleIds = overlappingRentals.Select(r => r.VoziloIdVozilo).Distinct().ToList();

                
                var availableVehicles = allVehicles.Where(v => !unavailableVehicleIds.Contains(v.IdVozilo)).ToList();

                _availableVehicles = new ObservableCollection<Vozilo>(availableVehicles);
                _vehiclesView = new ListCollectionView(_availableVehicles);
                VehiclesListView.ItemsSource = _vehiclesView;

                
            }
            catch (Exception ex)
            {
                ShowMessage(FindResource("Error").ToString(),
                    $"{FindResource("ErrorLoadingVehicles")}: {ex.Message}", MessageType.Error);
            }
        }



        private void TogglePricingButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedClient == null)
            {
                ShowMessage(FindResource("ValidationError").ToString(),
                    FindResource("SelectClient").ToString(), MessageType.Warning);
                return;
            }
            ShowPricingView();
        }

        private void BackToClientOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            HidePricingView();
        }

        private void FinalizeReservationButton_Click(object sender, RoutedEventArgs e)
        {
            
            ConfirmButton_Click(sender, e);
        }

        
        private void ShowPricingView()
        {
            _isPricingVisible = true;
            ClientOptionsGrid.Visibility = Visibility.Collapsed;
            PricingGrid.Visibility = Visibility.Visible;

            
            UpdatePricingSummary();
        }

        private void HidePricingView()
        {
            _isPricingVisible = false;
            ClientOptionsGrid.Visibility = Visibility.Visible;
            PricingGrid.Visibility = Visibility.Collapsed;
        }

        
        private void UpdatePricingSummary()
        {
            
            PricingSummaryVehicleText.Text = $"{_selectedVehicle.Marka} {_selectedVehicle.Model}, {_selectedVehicle.Godiste}";

            
            if (_selectedClient != null)
            {
                PricingSummaryClientText.Text = $"{_selectedClient.FullName}\n{_selectedClient.Email}\n{_selectedClient.Telefon}";
                FinalizeReservationButton.IsEnabled = true;
            }
            else
            {
                PricingSummaryClientText.Text = FindResource("NoClientSelected").ToString();
                FinalizeReservationButton.IsEnabled = false;
            }

            
            var selectedOptions = _availableOptions.Where(o => o.IsSelected).ToList();
            if (selectedOptions.Any())
            {
                StringBuilder sb = new StringBuilder();
                foreach (var option in selectedOptions)
                {
                    sb.AppendLine($"• {option.Naziv} - {string.Format(new CultureInfo("en-US"), "{0:C}", option.Cijena)}");
                }
                PricingSummaryOptionsText.Text = sb.ToString().TrimEnd();
            }
            else
            {
                PricingSummaryOptionsText.Text = FindResource("NoOptionsSelected").ToString();
            }
        }


        private void ReservationCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReservationCalendar.SelectedDates.Count > 0)
            {
                
                var selectedDates = ReservationCalendar.SelectedDates.OrderBy(d => d).ToList();
                _startDate = selectedDates.First();
                _endDate = selectedDates.Last();

                
                StartDateTextBox.Text = _startDate.ToShortDateString();
                EndDateTextBox.Text = _endDate.ToShortDateString();

                
                _totalDays = (_endDate - _startDate).Days + 1;
                TotalDaysTextBox.Text = _totalDays.ToString();

                
                FindVehiclesButton.IsEnabled = true;
            }
            else
            {
                
                StartDateTextBox.Text = string.Empty;
                EndDateTextBox.Text = string.Empty;
                TotalDaysTextBox.Text = string.Empty;
                _totalDays = 0;
                
            }
        }

        private void FindVehiclesButton_Click(object sender, RoutedEventArgs e)
        {
            if (_totalDays <= 0)
            {
                ShowMessage(FindResource("ValidationError").ToString(),
                    FindResource("SelectRentalDates").ToString(), MessageType.Warning);
                return;
            }

            
            LoadAvailableVehicles();

            
            SummaryDatesText.Text = $"{_startDate.ToShortDateString()} - {_endDate.ToShortDateString()}";
            SummaryTotalDaysTextBox.Text = _totalDays.ToString();

            
            DateSelectionGrid.Visibility = Visibility.Collapsed;
            VehicleSelectionGrid.Visibility = Visibility.Visible;
            ClientOptionsGrid.Visibility = Visibility.Collapsed;

            
            
        }

        private void BackToDateSelectionButton_Click(object sender, RoutedEventArgs e)
        {
            DateSelectionGrid.Visibility = Visibility.Visible;
            VehicleSelectionGrid.Visibility = Visibility.Collapsed;
            ClientOptionsGrid.Visibility = Visibility.Collapsed;

            
            _selectedVehicle = null;

            
            
        }

        private void VehiclesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedVehicle = VehiclesListView.SelectedItem as Vozilo;
            
            
        }

        private void SelectVehicleButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (_selectedVehicle == null)
            {
                ShowMessage(FindResource("ValidationError").ToString(),
                    FindResource("SelectVehiclePlease").ToString(), MessageType.Warning);
                return;
            }

            if (_selectedVehicle != null)
            {
                
                

                
                UpdateRateValue();

                
                UpdateTotalPrice();

                
                DateSelectionGrid.Visibility = Visibility.Collapsed;
                VehicleSelectionGrid.Visibility = Visibility.Collapsed;
                ClientOptionsGrid.Visibility = Visibility.Visible;
                PricingGrid.Visibility = Visibility.Collapsed;
                _isPricingVisible = false;

                
                
            }
        }

        private void VehicleSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_vehiclesView == null)
                return;

            string searchText = VehicleSearchTextBox.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                _vehiclesView.Filter = null;
            }
            else
            {
                _vehiclesView.Filter = item =>
                {
                    var vehicle = item as Vozilo;
                    return vehicle != null &&
                           (vehicle.Marka.ToLower().Contains(searchText) || vehicle.KategorijaVozilaIdKategorijaVozilaNavigation.Naziv.ToLower().Contains(searchText) ||
                            vehicle.Model.ToLower().Contains(searchText) ||
                            vehicle.Godiste.ToString().Contains(searchText));
                };
            }
        }

        private void ClientSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_clientsView == null)
                return;

            string searchText = ClientSearchTextBox.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                _clientsView.Filter = null;
            }
            else
            {
                _clientsView.Filter = item =>
                {
                    var client = item as ClientViewModel;
                    return client != null &&
                           (client.FullName.ToLower().Contains(searchText) ||
                            (client.Email != null && client.Email.ToLower().Contains(searchText)) ||
                            client.Telefon.Contains(searchText));
                };
            }
        }

        private void ClientsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedClient = ClientsListView.SelectedItem as ClientViewModel;

            
            bool canConfirm = _selectedClient != null && _selectedVehicle != null;
            
            FinalizeReservationButton.IsEnabled = canConfirm;

            
            if (_isPricingVisible)
            {
                UpdatePricingSummary();
            }
        }

        private void BackToVehiclesButton_Click(object sender, RoutedEventArgs e)
        {
            DateSelectionGrid.Visibility = Visibility.Collapsed;
            VehicleSelectionGrid.Visibility = Visibility.Visible;
            ClientOptionsGrid.Visibility = Visibility.Collapsed;

            
            
        }

        private void RateTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectedVehicle != null)
            {
                _isWeeklyRate = RateTypeComboBox.SelectedIndex == 1;
                UpdateRateValue();
                UpdateTotalPrice();
            }
        }

        private void UpdateRateValue()
        {
            if (_selectedVehicle != null)
            {
                decimal rate = _isWeeklyRate ? _selectedVehicle.SedmicnaTarifa : _selectedVehicle.DnevnaTarifa;
                RateValueTextBox.Text = string.Format(new CultureInfo("en-US"), "{0:C}", rate);
            }
        }

        private void UpdateTotalPrice()
        {
            if (_selectedVehicle == null || _totalDays <= 0)
            {
                _basePrice = 0;
            }
            else
            {
                if (_isWeeklyRate)
                {
                    
                    int fullWeeks = _totalDays / 7;
                    int remainingDays = _totalDays % 7;

                    _basePrice = (fullWeeks * _selectedVehicle.SedmicnaTarifa) + (remainingDays * _selectedVehicle.DnevnaTarifa);
                }
                else
                {
                    _basePrice = _totalDays * _selectedVehicle.DnevnaTarifa;
                }
            }

            
            decimal totalPrice = _basePrice + _optionsPrice;
            TotalPriceTextBlock.Text = string.Format(new CultureInfo("en-US"), "{0:C}", totalPrice);
        }

        private void Option_Checked(object sender, RoutedEventArgs e)
        {
            RecalculateOptionsPrice();
            if (_isPricingVisible)
            {
                UpdatePricingSummary();
            }
        }

        private void Option_Unchecked(object sender, RoutedEventArgs e)
        {
            RecalculateOptionsPrice();
            if (_isPricingVisible)
            {
                UpdatePricingSummary();
            }
        }

        private void RecalculateOptionsPrice()
        {
            _optionsPrice = _availableOptions.Where(o => o.IsSelected).Sum(o => o.Cijena);
            UpdateTotalPrice();
        }

        private void NewClientButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ClientDialog(null, _dbContext);
            if (dialog.ShowDialog() == true)
            {
                LoadClients();
                ShowMessage(FindResource("Success").ToString(), FindResource("ClientAdded").ToString(), MessageType.Success);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            

            ReservationCancelled?.Invoke(this, EventArgs.Empty);
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                if (_selectedVehicle == null)
                {
                    ShowMessage(FindResource("ValidationError").ToString(),
                        FindResource("SelectVehicle").ToString(), MessageType.Warning);
                    return;
                }

                if (_selectedClient == null)
                {
                    ShowMessage(FindResource("ValidationError").ToString(),
                        FindResource("SelectClient").ToString(), MessageType.Warning);
                    return;
                }

                decimal ukupno = decimal.Parse(TotalPriceTextBlock.Text.Replace("$", "").Replace(",", ""));
                if ( ((double)ukupno) > 9999.99)
                {
                    ShowMessage(FindResource("ValidationError").ToString(),
                        FindResource("PriceExceedsLimit").ToString(), MessageType.Warning);
                    return;
                }


                var rental = new Iznajmljivanje
                {
                    AgentKorisnikIdKorisnik = _currentUser.IdKorisnik,
                    VoziloIdVozilo = _selectedVehicle.IdVozilo,
                    VoziloKategorijaVozilaIdKategorijaVozila = _selectedVehicle.KategorijaVozilaIdKategorijaVozila,
                    KlijentIdKlijent = _selectedClient.IdKlijent,
                    DatumIznajmljivanja = DateOnly.FromDateTime(_startDate),
                    DatumVracanja = DateOnly.FromDateTime(_endDate),
                    Cijena = decimal.Parse(TotalPriceTextBlock.Text.Replace("$", "").Replace(",", "")),
                    DodatniOpis = AdditionalNotesTextBox.Text,
                    StatusIznajmljivanjaIdStatusIznajmljivanja = 1 
                };

                
                _dbContext.Iznajmljivanjes.Add(rental);
                _dbContext.SaveChanges();

                
                foreach (var option in _availableOptions.Where(o => o.IsSelected))
                {
                    var rentalOption = new OpcijaNaIznajmljivanju
                    {
                        IznajmljivanjeIdIznajmljivanje = rental.IdIznajmljivanje,
                        OpcijaIdOpcija = option.IdOpcija
                    };

                    _dbContext.OpcijaNaIznajmljivanjus.Add(rentalOption);
                }

                _dbContext.SaveChanges();

                
                ReservationCompleted?.Invoke(this, new ReservationCompletedEventArgs(rental));
            }
            catch (Exception ex)
            {
                ShowMessage(FindResource("Error").ToString(),
                    $"{FindResource("ErrorCreatingReservation")}: {ex.Message}", MessageType.Error);
            }
        }

        private void ShowMessage(string title, string message, MessageType type)
        {
            var messageBox = new CustomMessageBox(title, message, type);
            messageBox.ShowDialog();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ClientViewModel
    {
        public int IdKlijent { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
    }

    public class RentalOptionnViewModel : INotifyPropertyChanged
    {
        private bool _isSelected;

        public int IdOpcija { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public decimal Cijena { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public RentalOptionnViewModel(Opcija opcija)
        {
            IdOpcija = opcija.IdOpcija;
            Naziv = opcija.Naziv;
            Opis = opcija.Opis;
            Cijena = opcija.Cijena;
            IsSelected = false;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ReservationCompletedEventArgs : EventArgs
    {
        public Iznajmljivanje Reservation { get; private set; }

        public ReservationCompletedEventArgs(Iznajmljivanje reservation)
        {
            Reservation = reservation;
        }
    }
}