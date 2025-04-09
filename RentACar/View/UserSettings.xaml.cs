using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using RentACar.Model;

namespace RentACar.View
{
    public partial class UserSettings : UserControl
    {
        private readonly rentacarContext _dbContext;
        private readonly ObservableCollection<Telefon> _phoneNumbers;
        private Korisnik _currentUser;
        private Agent _currentAgent;

        public UserSettings()
        {
            InitializeComponent();
            _dbContext = new rentacarContext();
            _phoneNumbers = new ObservableCollection<Telefon>();

            
            PhonesListBox.ItemsSource = _phoneNumbers;

            
            LoadLocations();
        }

        private void LoadLocations()
        {
            try
            {
                LocationComboBox.ItemsSource = _dbContext.Mjestos.ToList();
            }
            catch (Exception ex)
            {
                ShowMessage(FindResource("Error").ToString(), ex.Message, MessageType.Error);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is Korisnik currentUser)
            {
                _currentUser = currentUser;

                
                UsernameTextBox.Text = currentUser.KorisnickoIme;
                EmailTextBox.Text = currentUser.Eposta;

                
                LocationComboBox.SelectedItem = _dbContext.Mjestos
                    .FirstOrDefault(m => m.IdMjesto == currentUser.MjestoIdMjesto);

                
                LoadPhoneNumbers();

                
                _currentAgent = _dbContext.Agents
                    .Include(a => a.KorisnikIdKorisnikNavigation)
                    .FirstOrDefault(a => a.KorisnikIdKorisnik == currentUser.IdKorisnik);

                if (_currentAgent != null)
                {
                    SalaryGrid.Visibility = Visibility.Visible;
                    SalaryTextBox.Text = _currentAgent.Plata?.ToString("0.00") ?? string.Empty;
                }
            }
        }

        private void LoadPhoneNumbers()
        {
            try
            {
                _phoneNumbers.Clear();

                var phones = _dbContext.Telefons
                    .Where(t => t.KorisnikIdKorisnik == _currentUser.IdKorisnik)
                    .ToList();

                foreach (var phone in phones)
                {
                    _phoneNumbers.Add(phone);
                }
            }
            catch (Exception ex)
            {
                ShowMessage(FindResource("Error").ToString(), ex.Message, MessageType.Error);
            }
        }

        private void SaveUserInfoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateUserInfo())
                    return;

                UpdateUserInfo();

                ShowMessage(FindResource("Success").ToString(),
                    FindResource("UserInfoUpdatedSuccessfully").ToString(),
                    MessageType.Success);
            }
            catch (Exception ex)
            {
                ShowMessage(FindResource("Error").ToString(), ex.Message, MessageType.Error);
            }
        }

        private bool ValidateUserInfo()
        {
            
            if (string.IsNullOrEmpty(EmailTextBox.Text) || !IsValidEmail(EmailTextBox.Text))
            {
                ShowMessage(FindResource("Error").ToString(),
                    FindResource("InvalidEmail").ToString(),
                    MessageType.Error);
                return false;
            }

            
            if (LocationComboBox.SelectedItem == null)
            {
                ShowMessage(FindResource("Error").ToString(),
                    FindResource("LocationRequired").ToString(),
                    MessageType.Error);
                return false;
            }

            
            if (SalaryGrid.Visibility == Visibility.Visible && !string.IsNullOrEmpty(SalaryTextBox.Text))
            {
                
                if (!Regex.IsMatch(SalaryTextBox.Text, @"^\d{1,4}(\.\d{1,2})?$"))
                {
                    ShowMessage(FindResource("Error").ToString(),
                        FindResource("InvalidSalary").ToString(),
                        MessageType.Error);
                    return false;
                }

                
                if (!decimal.TryParse(SalaryTextBox.Text, out decimal salary) ||
                    salary < 0 || salary > 9999.99m)
                {
                    ShowMessage(FindResource("Error").ToString(),
                        FindResource("InvalidSalary").ToString(),
                        MessageType.Error);
                    return false;
                }
            }

            
            if (EmailTextBox.Text != _currentUser.Eposta &&
                _dbContext.Korisniks.Any(k => k.Eposta == EmailTextBox.Text && k.IdKorisnik != _currentUser.IdKorisnik))
            {
                ShowMessage(FindResource("Error").ToString(),
                    FindResource("EmailExists").ToString(),
                    MessageType.Error);
                return false;
            }

            return true;
        }

        private void UpdateUserInfo()
        {
            
            _currentUser.Eposta = EmailTextBox.Text;
            _currentUser.MjestoIdMjesto = ((Mjesto)LocationComboBox.SelectedItem).IdMjesto;

            
            var existingPhones = _dbContext.Telefons
                .Where(t => t.KorisnikIdKorisnik == _currentUser.IdKorisnik)
                .ToList();

            foreach (var phone in existingPhones)
            {
                _dbContext.Telefons.Remove(phone);
            }

            _dbContext.SaveChanges();

            
            foreach (var phone in _phoneNumbers)
            {
                phone.KorisnikIdKorisnik = _currentUser.IdKorisnik;
                _dbContext.Telefons.Add(phone);
            }

            _dbContext.SaveChanges();
        }

        private void AddPhoneButton_Click(object sender, RoutedEventArgs e)
        {
            string phoneNumber = PhoneTextBox.Text.Trim();

            if (string.IsNullOrEmpty(phoneNumber))
            {
                ShowMessage(FindResource("Error").ToString(),
                    FindResource("PhoneNumberRequired").ToString(),
                    MessageType.Error);
                return;
            }

            
            if (!Regex.IsMatch(phoneNumber, @"^\+?[0-9]{8,15}$"))
            {
                ShowMessage(FindResource("Error").ToString(),
                    FindResource("InvalidPhoneNumber").ToString(),
                    MessageType.Error);
                return;
            }

            
            if (_phoneNumbers.Any(p => p.Telefon1 == phoneNumber))
            {
                ShowMessage(FindResource("Error").ToString(),
                    FindResource("PhoneNumberAlreadyExists").ToString(),
                    MessageType.Error);
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

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            string oldPassword = OldPasswordBox.Password;
            string newPassword = NewPasswordBox.Password;
            string confirmNewPassword = ConfirmNewPasswordBox.Password;

            
            if (!ValidateCurrentPassword(_currentUser, oldPassword))
            {
                ShowMessage(FindResource("Error").ToString(),
                    FindResource("IncorrectCurrentPassword").ToString(),
                    MessageType.Error);
                return;
            }

            
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                ShowMessage(FindResource("Error").ToString(),
                    FindResource("PasswordCannotBeEmpty").ToString(),
                    MessageType.Error);
                return;
            }

            
            if (newPassword != confirmNewPassword)
            {
                ShowMessage(FindResource("Error").ToString(),
                    FindResource("PasswordsDoNotMatch").ToString(),
                    MessageType.Error);
                return;
            }

            
            try
            {
                _currentUser.Lozinka = newPassword;
                _dbContext.Korisniks.Update(_currentUser);
                _dbContext.SaveChanges();

                
                OldPasswordBox.Clear();
                NewPasswordBox.Clear();
                ConfirmNewPasswordBox.Clear();

                ShowMessage(FindResource("Success").ToString(),
                    FindResource("PasswordChangedSuccessfully").ToString(),
                    MessageType.Success);
            }
            catch (Exception ex)
            {
                ShowMessage(FindResource("Error").ToString(),
                    ex.Message,
                    MessageType.Error);
            }
        }

        private bool ValidateCurrentPassword(Korisnik user, string inputPassword)
        {
            
            return inputPassword == user.Lozinka;
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

        private void ShowMessage(string title, string message, MessageType type)
        {
            var messageBox = new CustomMessageBox(title, message, type);
            messageBox.ShowDialog();
        }
    }
}