﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.EntityFrameworkCore;
using RentACar.Model;

namespace RentACar.View
{
    public partial class VehicleManagement : UserControl
    {
        private readonly rentacarContext _dbContext;
        private ObservableCollection<VehicleViewModel> _allVehicles;
        private ICollectionView _vehiclesView;
        private int _currentPage = 1;
        private int _itemsPerPage = 3;
        private int _totalPages;
        private string _searchText = string.Empty;
        private int? _selectedCategoryId;
        private bool _loadedCombo = false;
        private const string DEFAULT_IMAGE_PATH = "/Res/vehicleone.png";
        private const string VEHICLES_IMAGES_FOLDER = "VehicleImages";

        public VehicleManagement()
        {
            InitializeComponent();
            _dbContext = new rentacarContext();

            
            Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, VEHICLES_IMAGES_FOLDER));

            LoadCategories();
            LoadVehicles();
            UpdatePagination();
        }

        private void LoadCategories()
        {
            try
            {
                var categories = _dbContext.KategorijaVozilas.ToList();
                CategoryFilter.ItemsSource = categories;

                
                CategoryFilter.SelectedIndex = -1;
                _loadedCombo = true;
            }
            catch (Exception ex)
            {
                ShowMessage(FindResource("ErrorLoadingData").ToString(), ex.Message, MessageType.Error);
            }
        }

        private void LoadVehicles()
        {
            try
            {
                var vehicles = _dbContext.Vozilos
                    .Include(v => v.KategorijaVozilaIdKategorijaVozilaNavigation)
                    .ToList();

                _allVehicles = new ObservableCollection<VehicleViewModel>(
                    vehicles.Select(v => new VehicleViewModel(v, GetVehicleImagePath(v.IdVozilo)))
                );

                _vehiclesView = CollectionViewSource.GetDefaultView(_allVehicles);
                _vehiclesView.Filter = VehicleFilter;

                UpdateVehiclesDisplay();
            }
            catch (Exception ex)
            {
                ShowMessage(FindResource("ErrorLoadingData").ToString(), ex.Message, MessageType.Error);
            }
        }

        private string GetVehicleImagePath(int vehicleId)
        {
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, VEHICLES_IMAGES_FOLDER, $"{vehicleId}.jpg");
            return File.Exists(imagePath) ? imagePath : DEFAULT_IMAGE_PATH;
        }

        private bool VehicleFilter(object item)
        {
            if (!(item is VehicleViewModel vehicle))
                return false;

            
            if (_selectedCategoryId.HasValue)
            {
                if (vehicle.KategorijaVozilaIdKategorijaVozila != _selectedCategoryId.Value)
                    return false;
            }

            
            if (string.IsNullOrWhiteSpace(_searchText))
                return true;

            string searchText = _searchText.ToLower();
            return vehicle.Marka.ToLower().Contains(searchText) ||
                   vehicle.Model.ToLower().Contains(searchText) ||
                   vehicle.KategorijaVozilaIdKategorijaVozilaNavigation.Naziv.ToLower().Contains(searchText) ||
                   vehicle.Godiste.ToString().Contains(searchText);
        }

        private void UpdateVehiclesDisplay()
        {
            _vehiclesView.Refresh();

            var filteredVehicles = new List<VehicleViewModel>();
            foreach (VehicleViewModel vehicle in _vehiclesView)
            {
                filteredVehicles.Add(vehicle);
            }

            
            _totalPages = (int)Math.Ceiling(filteredVehicles.Count / (double)_itemsPerPage);

            
            if (_currentPage > _totalPages)
            {
                _currentPage = Math.Max(1, _totalPages);
            }

            
            var pagedVehicles = filteredVehicles
                .Skip((_currentPage - 1) * _itemsPerPage)
                .Take(_itemsPerPage)
                .ToList();

            VehiclesItemsControl.ItemsSource = pagedVehicles;

            UpdatePagination();
        }

        private void UpdatePagination()
        {
            
            PageCounter.Text = $"{_currentPage}/{_totalPages}";

            if (_totalPages <= 1)
            {
                
                PrevButton.Visibility = Visibility.Collapsed;
                NextButton.Visibility = Visibility.Collapsed;
                return;
            }

            
            PrevButton.Visibility = Visibility.Visible;
            NextButton.Visibility = Visibility.Visible;

            
            if (_currentPage <= 1)
            {
                PrevButton.Opacity = 0.5;
            }
            else
            {
                PrevButton.Opacity = 1;
            }

            PrevButton.IsEnabled = _currentPage > 1;

            if (_currentPage >= _totalPages)
            {
                NextButton.Opacity = 0.5;
            }
            else
            {
                NextButton.Opacity = 1;
            }

            NextButton.IsEnabled = _currentPage < _totalPages;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchText = SearchBox.Text;
            _currentPage = 1;
            UpdateVehiclesDisplay();
        }

        private void CategoryFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryFilter.SelectedValue is int categoryId)
            {
                _selectedCategoryId = categoryId;
                _currentPage = 1;
                if (_loadedCombo)
                {
                    UpdateVehiclesDisplay();
                    
                    ClearFilterButton.Visibility = Visibility.Visible;
                }
            }
        }

        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            CategoryFilter.SelectedIndex = -1;
            _selectedCategoryId = null;
            _currentPage = 1;
            UpdateVehiclesDisplay();
            ClearFilterButton.Visibility = Visibility.Collapsed;
        }

        private void AddVehicleButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VehicleDialog(null, _dbContext);
            if (dialog.ShowDialog() == true)
            {
                LoadVehicles();
                ShowMessage(FindResource("Success").ToString(), FindResource("VehicleAdded").ToString(), MessageType.Success);
            }
        }

        private void EditVehicleButton_Click(object sender, RoutedEventArgs e)
        {
            var vehicle = ((Button)sender).DataContext as VehicleViewModel;
            if (vehicle != null)
            {
                var originalVehicle = _dbContext.Vozilos
                    .Include(v => v.KategorijaVozilaIdKategorijaVozilaNavigation)
                    .FirstOrDefault(v => v.IdVozilo == vehicle.IdVozilo);

                if (originalVehicle != null)
                {
                    var dialog = new VehicleDialog(originalVehicle, _dbContext);
                    if (dialog.ShowDialog() == true)
                    {
                        LoadVehicles();
                        ShowMessage(FindResource("Success").ToString(), FindResource("VehicleUpdated").ToString(), MessageType.Success);
                    }
                }
            }
        }

        private void DeleteVehicleButton_Click(object sender, RoutedEventArgs e)
        {
            var vehicle = ((Button)sender).DataContext as VehicleViewModel;
            if (vehicle != null)
            {
                var dialogTitle = FindResource("ConfirmDelete").ToString();
                var dialogMessage = string.Format(FindResource("DeleteVehicleConfirmation").ToString(), $"{vehicle.Marka} {vehicle.Model}");

                var confirmDialog = new CustomMessageBox(dialogTitle, dialogMessage, MessageType.Question, true);
                if (confirmDialog.ShowDialog() == true)
                {
                    try
                    {
                        
                        var rentalCount = _dbContext.Iznajmljivanjes.Count(i => i.VoziloIdVozilo == vehicle.IdVozilo);
                        if (rentalCount > 0)
                        {
                            ShowMessage(
                                FindResource("Error").ToString(),
                                FindResource("VehicleInUse").ToString(),
                                MessageType.Error
                            );
                            return;
                        }

                        
                        var vehicleToDelete = _dbContext.Vozilos.Find(vehicle.IdVozilo);
                        if (vehicleToDelete != null)
                        {
                            _dbContext.Vozilos.Remove(vehicleToDelete);
                            _dbContext.SaveChanges();

                            
                            try
                            {
                                string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, VEHICLES_IMAGES_FOLDER, $"{vehicle.IdVozilo}.jpg");
                                if (File.Exists(imagePath))
                                {
                                    File.Delete(imagePath);
                                }
                            }
                            catch (Exception ex)
                            {
                                
                                Console.WriteLine($"Error deleting image: {ex.Message}");
                            }

                            LoadVehicles();
                            ShowMessage(FindResource("Success").ToString(), FindResource("VehicleDeleted").ToString(), MessageType.Success);
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowMessage(FindResource("Error").ToString(), ex.Message, MessageType.Error);
                    }
                }
            }
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                UpdateVehiclesDisplay();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                UpdateVehiclesDisplay();
            }
        }

        private void ShowMessage(string title, string message, MessageType type)
        {
            var messageBox = new CustomMessageBox(title, message, type);
            messageBox.ShowDialog();
        }
    }

    public class VehicleViewModel : INotifyPropertyChanged
    {
        public int IdVozilo { get; set; }
        public int KategorijaVozilaIdKategorijaVozila { get; set; }
        public string Marka { get; set; }
        public string Model { get; set; }
        public short Godiste { get; set; }
        public int Snaga { get; set; }
        public decimal DnevnaTarifa { get; set; }
        public decimal SedmicnaTarifa { get; set; }
        public KategorijaVozila KategorijaVozilaIdKategorijaVozilaNavigation { get; set; }

        private string _imagePath;
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                if (_imagePath != value)
                {
                    _imagePath = value;
                    OnPropertyChanged(nameof(ImagePath));
                }
            }
        }

        public string FullName => $"{Marka} {Model}";
        public string YearAndPower => $"{Godiste}, {Snaga} KS";
        public string PriceInfo => string.Format(new CultureInfo("en-US"), "{0:C} / {1:C}", DnevnaTarifa, SedmicnaTarifa);

        public event PropertyChangedEventHandler PropertyChanged;

        public VehicleViewModel(Vozilo vehicle, string imagePath)
        {
            IdVozilo = vehicle.IdVozilo;
            KategorijaVozilaIdKategorijaVozila = vehicle.KategorijaVozilaIdKategorijaVozila;
            Marka = vehicle.Marka;
            Model = vehicle.Model;
            Godiste = vehicle.Godiste;
            Snaga = vehicle.Snaga;
            DnevnaTarifa = vehicle.DnevnaTarifa;
            SedmicnaTarifa = vehicle.SedmicnaTarifa;
            KategorijaVozilaIdKategorijaVozilaNavigation = vehicle.KategorijaVozilaIdKategorijaVozilaNavigation;
            ImagePath = imagePath;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class PageViewModel
    {
        public int PageNumber { get; set; }
        public bool IsActive { get; set; }
    }

    
}