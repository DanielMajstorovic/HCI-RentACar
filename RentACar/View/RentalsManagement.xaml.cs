using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using RentACar.Model;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Collections.Generic;

namespace RentACar.View
{
    public partial class RentalsManagement : UserControl
    {
        private ObservableCollection<Iznajmljivanje> _rentals;
        private ICollectionView _rentalsView;
        private readonly rentacarContext _dbContext;
        private int? _selectedStatusId;

        public RentalsManagement()
        {
            InitializeComponent();
            _dbContext = new rentacarContext();
            LoadStatusFilter();
            LoadRentals();

            
            this.IsVisibleChanged += AgentRentalsManagement_IsVisibleChanged;
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


        private void ViewRentalButton_Click(object sender, RoutedEventArgs e)
        {
            var rental = ((Button)sender).DataContext as Iznajmljivanje;
            if (rental != null)
            {
                var dialog = new RentalDetailsDialog(rental);
                dialog.ShowDialog();
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