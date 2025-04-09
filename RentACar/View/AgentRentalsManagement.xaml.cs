using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using RentACar.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace RentACar.View
{
    public partial class AgentRentalsManagement : UserControl, INotifyPropertyChanged
    {
        private ObservableCollection<Iznajmljivanje> _rentals;
        private ICollectionView _rentalsView;
        private readonly rentacarContext _dbContext;
        private int? _selectedStatusId;
        private bool _isShowingList = true;

        private Korisnik _currentUser;

        public Korisnik CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged(nameof(CurrentUser));
            }
        }

        public bool IsShowingList
        {
            get { return _isShowingList; }
            set
            {
                if (_isShowingList != value)
                {
                    _isShowingList = value;
                    OnPropertyChanged(nameof(IsShowingList));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public AgentRentalsManagement()
        {
            InitializeComponent();
            this.DataContext = this;  
            _dbContext = new rentacarContext();
            LoadStatusFilter();
            LoadRentals();

            this.IsVisibleChanged += AgentRentalsManagement_IsVisibleChanged;
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is Korisnik currentUser)
            {
                _currentUser = currentUser;
                reservationMakerControl.CurrentUser = currentUser;

            }
        }


        private void AgentRentalsManagement_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool isVisible && isVisible)
            {
                LoadRentals();
            }
        }


        private void LoadStatusFilter()
        {
            try
            {
                var statuses = _dbContext.Statusiznajmljivanjas.ToList();
                StatusFilterComboBox.ItemsSource = statuses;

                StatusFilterComboBox.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                ShowMessage(FindResource("ErrorLoadingData").ToString(), ex.Message, MessageType.Error);
            }
        }

        private void StatusFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StatusFilterComboBox.SelectedValue is int statusId)
            {
                _selectedStatusId = statusId;
                ClearStatusFilterButton.Visibility = Visibility.Visible;
            }
            else
            {
                _selectedStatusId = null;
                ClearStatusFilterButton.Visibility = Visibility.Collapsed;
            }

            LoadRentals();
        }

        private void ClearStatusFilterButton_Click(object sender, RoutedEventArgs e)
        {
            StatusFilterComboBox.SelectedIndex = -1;
            _selectedStatusId = null;
            LoadRentals();
            ClearStatusFilterButton.Visibility = Visibility.Collapsed;
        }

        private void LoadRentals()
        {
            try
            {
                var query = _dbContext.Iznajmljivanjes
                    .Include(r => r.KlijentIdKlijentNavigation)
                    .Include(r => r.VoziloIdVoziloNavigation)
                    .Include(r => r.StatusIznajmljivanjaIdStatusIznajmljivanjaNavigation)
                    .AsQueryable();

                if (_selectedStatusId.HasValue && _selectedStatusId.Value != -1)
                {
                    query = query.Where(r => r.StatusIznajmljivanjaIdStatusIznajmljivanja == _selectedStatusId.Value);
                }

                _rentals = new ObservableCollection<Iznajmljivanje>(query.ToList());

                RentalsDataGrid.ItemsSource = _rentals;
                _rentalsView = CollectionViewSource.GetDefaultView(_rentals);
                _rentalsView.Filter = RentalFilter;
            }
            catch (Exception ex)
            {
                ShowMessage(FindResource("ErrorLoadingData").ToString(), ex.Message, MessageType.Error);
            }
        }

        public void RefreshRentals()
        {
            LoadRentals();
        }

        private bool RentalFilter(object item)
        {
            if (string.IsNullOrEmpty(SearchBox.Text))
                return true;

            var rental = (Iznajmljivanje)item;
            var searchText = SearchBox.Text.ToLower();

            if ((rental.KlijentIdKlijentNavigation?.Ime?.ToLower()?.Contains(searchText) ?? false) ||
                (rental.KlijentIdKlijentNavigation?.Prezime?.ToLower()?.Contains(searchText) ?? false))
                return true;

            if ((rental.VoziloIdVoziloNavigation?.Marka?.ToLower()?.Contains(searchText) ?? false) ||
                (rental.VoziloIdVoziloNavigation?.Model?.ToLower()?.Contains(searchText) ?? false))
                return true;

            if (rental.DatumIznajmljivanja.ToString().Contains(searchText) ||
                (rental.DatumVracanja?.ToString()?.Contains(searchText) ?? false))
                return true;

            if (rental.StatusIznajmljivanjaIdStatusIznajmljivanjaNavigation?.NazivStatusa?.ToLower()?.Contains(searchText) ?? false)
                return true;

            if (rental.Cijena.ToString().Contains(searchText))
                return true;

            if (rental.DodatniOpis?.ToLower()?.Contains(searchText) ?? false)
                return true;

            return false;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _rentalsView?.Refresh();
        }

        private void AddRentalButton_Click(object sender, RoutedEventArgs e)
        {
            IsShowingList = false;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            reservationMakerControl.Reset();
            IsShowingList = true;
        }

        private void ViewRentalButton_Click(object sender, RoutedEventArgs e)
        {
            var rental = ((Button)sender).DataContext as Iznajmljivanje;
            if (rental != null)
            {
                var dialog = new RentalDetailsDialog(rental);
                dialog.ShowDialog();
            }
        }

        private void EditRentalButton_Click(object sender, RoutedEventArgs e)
        {
            var rental = ((Button)sender).DataContext as Iznajmljivanje;
            if (rental != null)
            {
                var dialog = new RentalStatusDialog(rental, _dbContext);
                if (dialog.ShowDialog() == true)
                {
                    LoadRentals();
                    ShowMessage(FindResource("Success").ToString(), FindResource("RentalStatusUpdated").ToString(), MessageType.Success);
                }
            }
        }

        private void ReservationMaker_ReservationCancelled(object sender, EventArgs e)
        {
            IsShowingList = true;
            reservationMakerControl.Reset();
        }

        private void ReservationMaker_ReservationCompleted(object sender, ReservationCompletedEventArgs e)
        {

            Iznajmljivanje rezervacija = e.Reservation;

            ShowMessage(FindResource("Success").ToString(), FindResource("RentalAdded").ToString(), MessageType.Success);
            
            LoadRentals();
            reservationMakerControl.Reset();
            IsShowingList = true;
        }

        private void DeleteRentalButton_Click(object sender, RoutedEventArgs e)
        {
            var rental = ((Button)sender).DataContext as Iznajmljivanje;
            if (rental != null)
            {
                var dialogTitle = FindResource("ConfirmDelete").ToString();
                var dialogMessage = string.Format(
                    FindResource("DeleteRentalConfirmation").ToString(),
                    $"{rental.KlijentIdKlijentNavigation?.Ime} {rental.KlijentIdKlijentNavigation?.Prezime}",
                    rental.DatumIznajmljivanja.ToString("d")
                );

                var confirmDialog = new CustomMessageBox(dialogTitle, dialogMessage, MessageType.Question, true);
                if (confirmDialog.ShowDialog() == true)
                {
                    try
                    {
                        var rentalOptions = _dbContext.OpcijaNaIznajmljivanjus
                            .Where(o => o.IznajmljivanjeIdIznajmljivanje == rental.IdIznajmljivanje)
                            .ToList();

                        _dbContext.OpcijaNaIznajmljivanjus.RemoveRange(rentalOptions);
                        _dbContext.Iznajmljivanjes.Remove(rental);
                        _dbContext.SaveChanges();

                        LoadRentals();
                        ShowMessage(FindResource("Success").ToString(), FindResource("RentalDeleted").ToString(), MessageType.Success);
                    }
                    catch (Exception ex)
                    {
                        ShowMessage(FindResource("Error").ToString(), ex.Message, MessageType.Error);
                    }
                }
            }
        }

        private void RentalsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void ShowMessage(string title, string message, MessageType type)
        {
            var messageBox = new CustomMessageBox(title, message, type);
            messageBox.ShowDialog();
        }

        public void Dispose()
        {
            this.IsVisibleChanged -= AgentRentalsManagement_IsVisibleChanged;
        }
    }
}