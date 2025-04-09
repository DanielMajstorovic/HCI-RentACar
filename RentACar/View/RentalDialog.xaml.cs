using Microsoft.EntityFrameworkCore;
using RentACar.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RentACar.View
{
    public partial class RentalDialog : Window, INotifyPropertyChanged
    {
        private readonly rentacarContext _dbContext;
        private readonly Vozilo _selectedVehicle;
        private readonly Agent _currentAgent;
        private readonly List<Iznajmljivanje> _existingRentals;
        private ObservableCollection<RentalOptionViewModel> _availableOptions;
        private List<DateOnly> _unavailableDates;
        private DateTime _startDate;
        private DateTime _endDate;
        private decimal _basePrice = 0;
        private decimal _optionsPrice = 0;
        private bool _isWeeklyRate = false;
        private int _totalDays = 0;

        public event PropertyChangedEventHandler PropertyChanged;

        public RentalDialog(Vozilo vehicle, Agent agent, List<Iznajmljivanje> existingRentals)
        {
            InitializeComponent();

            _dbContext = new rentacarContext();
            _selectedVehicle = vehicle;
            _currentAgent = agent;
            _existingRentals = existingRentals;

            
            VehicleInfoText.Text = $"{vehicle.Marka} {vehicle.Model}, {vehicle.Godiste}";

            
            RentalCalendar.DisplayDateStart = DateTime.Today;

            
            LoadUnavailableDates();

            
            LoadClients();

            
            LoadOptions();

            
            RateTypeComboBox.SelectedIndex = 0;

            
            UpdateRateValue();
        }

        private void LoadUnavailableDates()
        {
            
            _unavailableDates = new List<DateOnly>();

            
            foreach (var rental in _existingRentals)
            {
                
                if (rental.DatumVracanja.HasValue)
                {
                    
                    var currentDate = rental.DatumIznajmljivanja;
                    var endDate = rental.DatumVracanja.Value;

                    while (currentDate <= endDate)
                    {
                        _unavailableDates.Add(currentDate);
                        currentDate = currentDate.AddDays(1);
                    }
                }
                else
                {
                    
                    _unavailableDates.Add(rental.DatumIznajmljivanja);
                }
            }

            
            RentalCalendar.BlackoutDates.Clear();
            foreach (var dateGroup in _unavailableDates.GroupBy(d => new { d.Year, d.Month, d.Day }).Select(g => g.First()))
            {
                DateTime date = new DateTime(dateGroup.Year, dateGroup.Month, dateGroup.Day);
                RentalCalendar.BlackoutDates.Add(new CalendarDateRange(date));
            }

            
            RentalCalendar.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, DateTime.Today.AddDays(-1)));
        }

        private void LoadClients()
        {
            try
            {
                var clients = _dbContext.Klijents.ToList();
                ClientComboBox.ItemsSource = clients;

                if (clients.Any())
                {
                    ClientComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                var messageBox = new CustomMessageBox(FindResource("Error").ToString(),
        $"{FindResource("ErrorLoadingClients").ToString()}: {ex.Message}", MessageType.Error);
                messageBox.ShowDialog();
            }
        }

        private void LoadOptions()
        {
            try
            {
                var options = _dbContext.Opcijas.ToList();
                _availableOptions = new ObservableCollection<RentalOptionViewModel>(
                    options.Select(o => new RentalOptionViewModel(o))
                );

                OptionsItemsControl.ItemsSource = _availableOptions;
            }
            catch (Exception ex)
            {
                var messageBox = new CustomMessageBox(FindResource("Error").ToString(),
        $"{FindResource("ErrorLoadingOptions").ToString()}: {ex.Message}", MessageType.Error);
                messageBox.ShowDialog();
            }
        }

        private void RentalCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RentalCalendar.SelectedDates.Count > 0)
            {
                
                var selectedDates = RentalCalendar.SelectedDates.OrderBy(d => d).ToList();
                _startDate = selectedDates.First();
                _endDate = selectedDates.Last();

                
                bool hasBlackoutDate = false;
                for (DateTime date = _startDate; date <= _endDate; date = date.AddDays(1))
                {
                    if (IsDateBlackedOut(date))
                    {
                        hasBlackoutDate = true;
                        break;
                    }
                }

                
                if (hasBlackoutDate)
                {
                    RentalCalendar.SelectedDates.Clear();
                    var messageBox = new CustomMessageBox(
                        FindResource("ValidationError").ToString(),
                        FindResource("CannotSelectUnavailableDates").ToString(),
                        MessageType.Warning);
                    messageBox.ShowDialog();
                    return;
                }

                
                StartDateTextBox.Text = _startDate.ToShortDateString();
                EndDateTextBox.Text = _endDate.ToShortDateString();

                
                _totalDays = (_endDate - _startDate).Days + 1;
                TotalDaysTextBox.Text = _totalDays.ToString();

                
                UpdateTotalPrice();
            }
            else
            {
                
                StartDateTextBox.Text = string.Empty;
                EndDateTextBox.Text = string.Empty;
                TotalDaysTextBox.Text = string.Empty;
                _totalDays = 0;
                UpdateTotalPrice();
            }
        }

        
        private bool IsDateBlackedOut(DateTime date)
        {
            
            foreach (CalendarDateRange range in RentalCalendar.BlackoutDates)
            {
                if (date >= range.Start && date <= range.End)
                {
                    return true;
                }
            }
            return false;
        }

        private void RateTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _isWeeklyRate = RateTypeComboBox.SelectedIndex == 1;
            UpdateRateValue();
            UpdateTotalPrice();
        }

        private void UpdateRateValue()
        {
            decimal rate = _isWeeklyRate ? _selectedVehicle.SedmicnaTarifa : _selectedVehicle.DnevnaTarifa;
            
            RateValueTextBox.Text = string.Format(new CultureInfo("en-US"), "{0:C}", rate);
        }

        private void UpdateTotalPrice()
        {
            
            if (_totalDays > 0)
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
            else
            {
                _basePrice = 0;
            }

            
            decimal totalPrice = _basePrice + _optionsPrice;
            
            TotalPriceTextBlock.Text = string.Format(new CultureInfo("en-US"), "{0:C}", totalPrice);
        }

        private void Option_Checked(object sender, RoutedEventArgs e)
        {
            RecalculateOptionsPrice();
        }

        private void Option_Unchecked(object sender, RoutedEventArgs e)
        {
            RecalculateOptionsPrice();
        }

        private void RecalculateOptionsPrice()
        {
            _optionsPrice = _availableOptions.Where(o => o.IsSelected).Sum(o => o.Cijena);
            UpdateTotalPrice();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                if (RentalCalendar.SelectedDates.Count == 0)
                {
                    var messageBox = new CustomMessageBox(FindResource("ValidationError").ToString(),
         FindResource("SelectRentalDates").ToString(), MessageType.Warning);
                    messageBox.ShowDialog();
                    return;
                }

                if (ClientComboBox.SelectedItem == null)
                {
                    var messageBox = new CustomMessageBox(FindResource("ValidationError").ToString(),
        FindResource("SelectClient").ToString(), MessageType.Warning);
                    messageBox.ShowDialog();
                    return;
                }

                decimal ukupno = decimal.Parse(TotalPriceTextBlock.Text.Replace("$", "").Replace(",", ""));
                if (((double)ukupno) > 9999.99)
                {
                    var messageBox = new CustomMessageBox(FindResource("ValidationError").ToString(),
                        FindResource("PriceExceedsLimit").ToString(), MessageType.Warning);
                    messageBox.ShowDialog();
                    return;
                }


                var rental = new Iznajmljivanje
                {
                    AgentKorisnikIdKorisnik = _currentAgent.KorisnikIdKorisnik,
                    VoziloIdVozilo = _selectedVehicle.IdVozilo,
                    VoziloKategorijaVozilaIdKategorijaVozila = _selectedVehicle.KategorijaVozilaIdKategorijaVozila,
                    KlijentIdKlijent = ((Klijent)ClientComboBox.SelectedItem).IdKlijent,
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

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                var messageBox = new CustomMessageBox(FindResource("Error").ToString(),
         $"{FindResource("ErrorCreatingRental").ToString()}: {ex.Message}", MessageType.Error);
                messageBox.ShowDialog();
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RentalOptionViewModel : INotifyPropertyChanged
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

        public RentalOptionViewModel(Opcija opcija)
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
}

