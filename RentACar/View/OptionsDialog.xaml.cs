using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using RentACar.Model;

namespace RentACar.View
{
    public partial class OptionDialog : Window
    {
        private readonly rentacarContext _dbContext;
        private readonly Opcija _optionToEdit;
        private readonly bool _isEditMode;

        public OptionDialog(Opcija optionToEdit, rentacarContext dbContext)
        {
            InitializeComponent();
            _dbContext = dbContext;
            _optionToEdit = optionToEdit;
            _isEditMode = optionToEdit != null;

            
            DialogTitle.Text = _isEditMode
                ? FindResource("EditOption").ToString()
                : FindResource("AddOption").ToString();

            
            if (_isEditMode)
            {
                FillFormWithOptionData();
            }
        }

        private void FillFormWithOptionData()
        {
            NameTextBox.Text = _optionToEdit.Naziv;
            DescriptionTextBox.Text = _optionToEdit.Opis;
            PriceTextBox.Text = _optionToEdit.Cijena.ToString("0.00");
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateInput())
                    return;

                if (_isEditMode)
                {
                    UpdateExistingOption();
                }
                else
                {
                    CreateNewOption();
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
            
            if (string.IsNullOrEmpty(NameTextBox.Text))
            {
                ShowError(FindResource("NameRequired").ToString());
                return false;
            }

            
            if (string.IsNullOrEmpty(PriceTextBox.Text) ||
                !Regex.IsMatch(PriceTextBox.Text, @"^\d{1,3}(\.\d{1,2})?$") || 
                !decimal.TryParse(PriceTextBox.Text, out decimal price) ||
                price < 0 || price > 999.99m) 
            {
                ShowError(FindResource("InvalidPrice").ToString());
                return false;
            }

            
            string currentName = _isEditMode ? _optionToEdit.Naziv : null;
            if (NameTextBox.Text != currentName &&
                _dbContext.Opcijas.Any(o => o.Naziv == NameTextBox.Text))
            {
                ShowError(FindResource("OptionNameExists").ToString());
                return false;
            }

            return true;
        }

        private void UpdateExistingOption()
        {
            _optionToEdit.Naziv = NameTextBox.Text;
            _optionToEdit.Opis = DescriptionTextBox.Text;
            _optionToEdit.Cijena = decimal.Parse(PriceTextBox.Text);

            _dbContext.Opcijas.Update(_optionToEdit);
            _dbContext.SaveChanges();
        }

        private void CreateNewOption()
        {
            var newOption = new Opcija
            {
                Naziv = NameTextBox.Text,
                Opis = DescriptionTextBox.Text,
                Cijena = decimal.Parse(PriceTextBox.Text)
            };

            _dbContext.Opcijas.Add(newOption);
            _dbContext.SaveChanges();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
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