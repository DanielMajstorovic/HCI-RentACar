using System;
using System.Windows;
using System.Text.RegularExpressions;
using RentACar.Model;

namespace RentACar.View
{

    public partial class ClientDialog : Window
    {
        private readonly rentacarContext _dbContext;
        private readonly Klijent _client;
        private readonly bool _isEditMode;

        
        public ClientDialog(Klijent client, rentacarContext dbContext)
        {
            InitializeComponent();
            _dbContext = dbContext;

            _isEditMode = client != null;

            if (_isEditMode)
            {
                _client = client;
                DialogTitle.Text = (string)FindResource("EditClient");
                LoadClientData();
            }
            else
            {
                _client = new Klijent();
                DialogTitle.Text = (string)FindResource("AddClient");
            }

            FirstNameTextBox.Focus();
        }

       
        private void LoadClientData()
        {
            FirstNameTextBox.Text = _client.Ime;
            LastNameTextBox.Text = _client.Prezime;
            PhoneTextBox.Text = _client.BrojTelefona;
            EmailTextBox.Text = _client.Email;
        }

        
        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
            {
                ShowError(FindResource("FirstNameRequired").ToString());
                FirstNameTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text))
            {
                ShowError(FindResource("LastNameRequired").ToString());
                LastNameTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(PhoneTextBox.Text))
            {
                ShowError(FindResource("PhoneNumberRequired").ToString());
                PhoneTextBox.Focus();
                return false;
            }

            if (!IsValidPhoneNumber(PhoneTextBox.Text))
            {
                ShowError(FindResource("InvalidPhoneNumber").ToString());
                PhoneTextBox.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(EmailTextBox.Text) && !IsValidEmail(EmailTextBox.Text))
            {
                ShowError(FindResource("InvalidEmail").ToString());
                EmailTextBox.Focus();
                return false;
            }

            return true;
        }

        
        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^[\d\s\-\(\)]+$");
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

      
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
                return;

            try
            {
                _client.Ime = FirstNameTextBox.Text.Trim();
                _client.Prezime = LastNameTextBox.Text.Trim();
                _client.BrojTelefona = PhoneTextBox.Text.Trim();
                _client.Email = string.IsNullOrWhiteSpace(EmailTextBox.Text) ? null : EmailTextBox.Text.Trim();

                if (!_isEditMode)
                {
                    _dbContext.Klijents.Add(_client);
                }

                _dbContext.SaveChanges();

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

       
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}