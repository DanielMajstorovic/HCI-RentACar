using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using RentACar.Model;
using System.Text.RegularExpressions;

namespace RentACar.View
{
    public partial class AgentDialog : Window
    {
        private readonly rentacarContext _dbContext;
        private readonly Agent _agentToEdit;
        private readonly ObservableCollection<Telefon> _phoneNumbers;
        private bool _isEditMode;

        public AgentDialog(Agent agentToEdit, rentacarContext dbContext)
        {
            InitializeComponent();
            _dbContext = dbContext;
            _agentToEdit = agentToEdit;
            _isEditMode = agentToEdit != null;
            _phoneNumbers = new ObservableCollection<Telefon>();

            DialogTitle.Text = _isEditMode
                ? FindResource("EditAgent").ToString()
                : FindResource("AddAgent").ToString();

            LoadLocations();

            PhonesListBox.ItemsSource = _phoneNumbers;

            if (_isEditMode)
            {
                FillFormWithAgentData();
            }
        }

        private void LoadLocations()
        {
            try
            {
                LocationComboBox.ItemsSource = _dbContext.Mjestos.ToList();

                if (!_isEditMode && LocationComboBox.Items.Count > 0)
                {
                    LocationComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void FillFormWithAgentData()
        {
            UsernameTextBox.Text = _agentToEdit.KorisnikIdKorisnikNavigation.KorisnickoIme;
            EmailTextBox.Text = _agentToEdit.KorisnikIdKorisnikNavigation.Eposta;
            SalaryTextBox.Text = _agentToEdit.Plata?.ToString("0.00") ?? string.Empty;

            LocationComboBox.SelectedItem = _dbContext.Mjestos.FirstOrDefault(
                m => m.IdMjesto == _agentToEdit.KorisnikIdKorisnikNavigation.MjestoIdMjesto);

            foreach (var phone in _agentToEdit.KorisnikIdKorisnikNavigation.Telefons)
            {
                _phoneNumbers.Add(phone);
            }

            PasswordBox.Password = string.Empty;
        }

        private void AddPhoneButton_Click(object sender, RoutedEventArgs e)
        {
            string phoneNumber = PhoneTextBox.Text.Trim();

            if (string.IsNullOrEmpty(phoneNumber))
            {
                ShowError(FindResource("PhoneNumberRequired").ToString());
                return;
            }

            if (!Regex.IsMatch(phoneNumber, @"^\+?[0-9]{8,15}$"))
            {
                ShowError(FindResource("InvalidPhoneNumber").ToString());
                return;
            }

            if (_phoneNumbers.Any(p => p.Telefon1 == phoneNumber))
            {
                ShowError(FindResource("PhoneNumberAlreadyExists").ToString());
                return;
            }

            _phoneNumbers.Add(new Telefon { Telefon1 = phoneNumber });
            PhoneTextBox.Clear();
        }

        private void RemovePhoneButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Telefon phone)
            {
                _phoneNumbers.Remove(phone);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateInput())
                    return;

                if (_isEditMode)
                {
                    UpdateExistingAgent();
                }
                else
                {
                    CreateNewAgent();
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrEmpty(UsernameTextBox.Text))
            {
                ShowError(FindResource("UsernameRequired").ToString());
                return false;
            }

            if (!_isEditMode && string.IsNullOrEmpty(PasswordBox.Password))
            {
                ShowError(FindResource("PasswordRequired").ToString());
                return false;
            }

            if (string.IsNullOrEmpty(EmailTextBox.Text) || !IsValidEmail(EmailTextBox.Text))
            {
                ShowError(FindResource("InvalidEmail").ToString());
                return false;
            }

            if (LocationComboBox.SelectedItem == null)
            {
                ShowError(FindResource("LocationRequired").ToString());
                return false;
            }

            if (!string.IsNullOrEmpty(SalaryTextBox.Text))
            {
                
                if (!Regex.IsMatch(SalaryTextBox.Text, @"^\d{1,4}(\.\d{1,2})?$"))
                {
                    ShowError(FindResource("InvalidSalary").ToString());
                    return false;
                }

                if (!decimal.TryParse(SalaryTextBox.Text, out decimal salary) ||
                    salary < 0 || salary > 9999.99m)
                {
                    ShowError(FindResource("InvalidSalary").ToString());
                    return false;
                }
            }

            if (!_isEditMode && _dbContext.Korisniks.Any(k => k.KorisnickoIme == UsernameTextBox.Text))
            {
                ShowError(FindResource("UsernameExists").ToString());
                return false;
            }

            string currentEmail = _isEditMode ? _agentToEdit.KorisnikIdKorisnikNavigation.Eposta : null;
            if (EmailTextBox.Text != currentEmail &&
                _dbContext.Korisniks.Any(k => k.Eposta == EmailTextBox.Text))
            {
                ShowError(FindResource("EmailExists").ToString());
                return false;
            }

            return true;
        }

        private void UpdateExistingAgent()
        {
            var korisnik = _agentToEdit.KorisnikIdKorisnikNavigation;
            korisnik.KorisnickoIme = UsernameTextBox.Text;
            korisnik.Eposta = EmailTextBox.Text;
            korisnik.MjestoIdMjesto = ((Mjesto)LocationComboBox.SelectedItem).IdMjesto;

            if (!string.IsNullOrEmpty(PasswordBox.Password))
            {
                korisnik.Lozinka = PasswordBox.Password; 
            }

            _agentToEdit.Plata = string.IsNullOrEmpty(SalaryTextBox.Text)
                ? null
                : decimal.Parse(SalaryTextBox.Text);

            var existingPhones = _dbContext.Telefons
                .Where(t => t.KorisnikIdKorisnik == korisnik.IdKorisnik)
                .ToList();

            foreach (var phone in existingPhones)
            {
                _dbContext.Telefons.Remove(phone);
            }

            _dbContext.SaveChanges();

            foreach (var phone in _phoneNumbers)
            {
                phone.KorisnikIdKorisnik = korisnik.IdKorisnik;
                _dbContext.Telefons.Add(phone);
            }

            _dbContext.SaveChanges();
        }

        private void CreateNewAgent()
        {
            var korisnik = new Korisnik
            {
                KorisnickoIme = UsernameTextBox.Text,
                Lozinka = PasswordBox.Password, 
                Eposta = EmailTextBox.Text,
                TipNaloga = "Agent",
                MjestoIdMjesto = ((Mjesto)LocationComboBox.SelectedItem).IdMjesto
            };

            _dbContext.Korisniks.Add(korisnik);
            _dbContext.SaveChanges();

            var agent = new Agent
            {
                KorisnikIdKorisnik = korisnik.IdKorisnik,
                Plata = string.IsNullOrEmpty(SalaryTextBox.Text)
                    ? null
                    : decimal.Parse(SalaryTextBox.Text)
            };

            _dbContext.Agents.Add(agent);

            foreach (var phone in _phoneNumbers)
            {
                phone.KorisnikIdKorisnik = korisnik.IdKorisnik;
                _dbContext.Telefons.Add(phone);
            }

            _dbContext.SaveChanges();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void ShowError(string message)
        {
            var messageBox = new CustomMessageBox(
                FindResource("Error").ToString(),
                message,
                MessageType.Error);
            messageBox.ShowDialog();
        }
    }
}