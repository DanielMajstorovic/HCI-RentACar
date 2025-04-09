using System;
using System.Linq;
using System.Windows;
using RentACar.Model;
using Microsoft.EntityFrameworkCore;

namespace RentACar.View
{
    public partial class RentalStatusDialog : Window
    {
        private readonly Iznajmljivanje _rental;
        private readonly rentacarContext _dbContext;
        private bool _isDataModified = false;

        public RentalStatusDialog(Iznajmljivanje rental, rentacarContext dbContext)
        {
            InitializeComponent();
            _rental = rental;
            _dbContext = dbContext;
            LoadRentalData();
        }

        private void LoadRentalData()
        {
            
            string clientName = _rental.KlijentIdKlijentNavigation?.ToString() ?? "";
            string vehicleName = $"{_rental.VoziloIdVoziloNavigation?.Marka} {_rental.VoziloIdVoziloNavigation?.Model}";
            string rentalDate = _rental.DatumIznajmljivanja.ToString("dd/MM/yyyy");
            string returnDate = _rental.DatumVracanja.HasValue ? _rental.DatumVracanja.Value.ToString("dd/MM/yyyy") : FindResource("NotReturned").ToString();

            RentalSummaryTextBlock.Text = $"{clientName} - {vehicleName} {FindResource("FromDate").ToString()} {rentalDate} {FindResource("ToDate").ToString()} {returnDate}";

            
            var statuses = _dbContext.Statusiznajmljivanjas.ToList();
            StatusComboBox.ItemsSource = statuses;

            
            StatusComboBox.SelectedValue = _rental.StatusIznajmljivanjaIdStatusIznajmljivanja;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                var selectedStatus = StatusComboBox.SelectedItem as Statusiznajmljivanja;
                if (selectedStatus == null)
                {
                    ShowMessage(FindResource("Error").ToString(), FindResource("SelectStatus").ToString(), MessageType.Error);
                    return;
                }

                
                if (_rental.StatusIznajmljivanjaIdStatusIznajmljivanja != selectedStatus.IdStatusIznajmljivanja)
                {
                    _isDataModified = true;
                    _rental.StatusIznajmljivanjaIdStatusIznajmljivanja = selectedStatus.IdStatusIznajmljivanja;

                    
                    _dbContext.Update(_rental);
                    _dbContext.SaveChanges();

                    DialogResult = true;
                }
                else
                {
                    DialogResult = false;
                }

                Close();
            }
            catch (Exception ex)
            {
                ShowMessage(FindResource("Error").ToString(), $"{FindResource("ErrorSaving").ToString()}: {ex.Message}", MessageType.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ShowMessage(string title, string message, MessageType type)
        {
            var messageBox = new CustomMessageBox(title, message, type);
            messageBox.ShowDialog();
        }
    }
}