using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using RentACar.Model;

namespace RentACar.View
{
    public partial class VehicleDialog : Window
    {
        private readonly rentacarContext _dbContext;
        private readonly Vozilo _vehicle;
        private bool _isEditMode;
        private string _selectedImagePath;
        private const string VEHICLES_IMAGES_FOLDER = "VehicleImages";

        public VehicleDialog(Vozilo vehicle, rentacarContext dbContext)
        {
            InitializeComponent();
            _dbContext = dbContext;
            _vehicle = vehicle ?? new Vozilo();
            _isEditMode = vehicle != null && vehicle.IdVozilo > 0;

            
            DialogTitle.Text = _isEditMode
                ? FindResource("EditVehicle").ToString()
                : FindResource("AddVehicle").ToString();

            LoadCategories();
            LoadVehicleData();
        }

        private void LoadCategories()
        {
            try
            {
                var categories = _dbContext.KategorijaVozilas.ToList();
                CategoryComboBox.ItemsSource = categories;

                if (_isEditMode && _vehicle.KategorijaVozilaIdKategorijaVozila > 0)
                {
                    CategoryComboBox.SelectedValue = _vehicle.KategorijaVozilaIdKategorijaVozila;
                }
            }
            catch (Exception ex)
            {
                ShowMessage(FindResource("Error").ToString(), ex.Message, MessageType.Error);
            }
        }

        private void LoadVehicleData()
        {
            if (_isEditMode)
            {
                BrowseImageButton.Visibility = Visibility.Collapsed;
                BrandTextBox.Text = _vehicle.Marka;
                ModelTextBox.Text = _vehicle.Model;
                YearTextBox.Text = _vehicle.Godiste.ToString();
                PowerTextBox.Text = _vehicle.Snaga.ToString();
                DailyRateTextBox.Text = _vehicle.DnevnaTarifa.ToString("F2");
                WeeklyRateTextBox.Text = _vehicle.SedmicnaTarifa.ToString("F2");

                
                string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, VEHICLES_IMAGES_FOLDER, $"{_vehicle.IdVozilo}.jpg");
                if (File.Exists(imagePath))
                {
                    LoadImage(imagePath);
                    _selectedImagePath = imagePath;
                }
                else
                {
                    
                    LoadImage("/Res/default_car.png");
                }
            }
            else
            {
                
                LoadImage("/Res/default_car.png");
            }
        }

        private void LoadImage(string path)
        {
            try
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(path.StartsWith("/") ? path : path, UriKind.RelativeOrAbsolute);
                image.EndInit();
                VehicleImage.Source = image;
            }
            catch (Exception)
            {
                
                try
                {
                    BitmapImage defaultImage = new BitmapImage();
                    defaultImage.BeginInit();
                    defaultImage.CacheOption = BitmapCacheOption.OnLoad;
                    defaultImage.UriSource = new Uri("/Res/default_car.png", UriKind.Relative);
                    defaultImage.EndInit();
                    VehicleImage.Source = defaultImage;
                }
                catch
                {
                    
                }
            }
        }

        private void BrowseImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png",
                Title = FindResource("SelectImage").ToString()
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedImagePath = openFileDialog.FileName;
                LoadImage(_selectedImagePath);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                try
                {
                    
                    _vehicle.KategorijaVozilaIdKategorijaVozila = (int)CategoryComboBox.SelectedValue;
                    _vehicle.Marka = BrandTextBox.Text.Trim();
                    _vehicle.Model = ModelTextBox.Text.Trim();
                    _vehicle.Godiste = short.Parse(YearTextBox.Text.Trim());
                    _vehicle.Snaga = int.Parse(PowerTextBox.Text.Trim());
                    _vehicle.DnevnaTarifa = decimal.Parse(DailyRateTextBox.Text.Trim());
                    _vehicle.SedmicnaTarifa = decimal.Parse(WeeklyRateTextBox.Text.Trim());

                    if (!_isEditMode)
                    {
                        
                        _dbContext.Vozilos.Add(_vehicle);
                    }
                    else
                    {
                        
                        _dbContext.Vozilos.Update(_vehicle);
                    }

                    _dbContext.SaveChanges();

                    
                    if (!string.IsNullOrEmpty(_selectedImagePath) && _selectedImagePath != "/Res/default_car.png")
                    {
                        SaveVehicleImage(_vehicle.IdVozilo, _selectedImagePath);
                    }

                    DialogResult = true;
                    Close();
                }
                catch (Exception ex)
                {
                    ShowMessage(FindResource("Error").ToString(), ex.Message, MessageType.Error);
                }
            }
        }

        private void SaveVehicleImage(int vehicleId, string sourcePath)
        {
            try
            {
                
                string targetDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, VEHICLES_IMAGES_FOLDER);
                Directory.CreateDirectory(targetDirectory);

                string targetPath = Path.Combine(targetDirectory, $"{vehicleId}.jpg");

                
                if (File.Exists(sourcePath) && !sourcePath.StartsWith("/"))
                {
                    BitmapImage sourceImage = new BitmapImage(new Uri(sourcePath));
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(sourceImage));

                    using (FileStream fileStream = new FileStream(targetPath, FileMode.Create))
                    {
                        encoder.Save(fileStream);
                    }
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error saving image: {ex.Message}");
            }
        }

        private bool ValidateInput()
        {
            
            if (CategoryComboBox.SelectedValue == null)
            {
                ShowMessage(FindResource("ValidationError").ToString(),
                    FindResource("CategoryRequired").ToString(),
                    MessageType.Warning);
                CategoryComboBox.Focus();
                return false;
            }

            
            if (string.IsNullOrWhiteSpace(BrandTextBox.Text))
            {
                ShowMessage(FindResource("ValidationError").ToString(),
                    FindResource("BrandRequired").ToString(),
                    MessageType.Warning);
                BrandTextBox.Focus();
                return false;
            }

            
            if (string.IsNullOrWhiteSpace(ModelTextBox.Text))
            {
                ShowMessage(FindResource("ValidationError").ToString(),
                    FindResource("ModelRequired").ToString(),
                    MessageType.Warning);
                ModelTextBox.Focus();
                return false;
            }

            
            if (string.IsNullOrWhiteSpace(YearTextBox.Text) || !short.TryParse(YearTextBox.Text, out short year) ||
                year < 1900 || year > DateTime.Now.Year)
            {
                ShowMessage(FindResource("ValidationError").ToString(),
                    FindResource("YearInvalid").ToString(),
                    MessageType.Warning);
                YearTextBox.Focus();
                return false;
            }

            
            if (string.IsNullOrWhiteSpace(PowerTextBox.Text) || !int.TryParse(PowerTextBox.Text, out int power) ||
                power <= 0)
            {
                ShowMessage(FindResource("ValidationError").ToString(),
                    FindResource("PowerInvalid").ToString(),
                    MessageType.Warning);
                PowerTextBox.Focus();
                return false;
            }

            
            if (string.IsNullOrWhiteSpace(DailyRateTextBox.Text) ||
                !decimal.TryParse(DailyRateTextBox.Text, out decimal dailyRate) ||
                dailyRate <= 0 ||
                dailyRate >= 1000 ||  
                Math.Round(dailyRate, 2) != dailyRate) 
            {
                ShowMessage(FindResource("ValidationError").ToString(),
                    FindResource("DailyRateInvalid").ToString(),
                    MessageType.Warning);
                DailyRateTextBox.Focus();
                return false;
            }

            
            if (string.IsNullOrWhiteSpace(WeeklyRateTextBox.Text) ||
                !decimal.TryParse(WeeklyRateTextBox.Text, out decimal weeklyRate) ||
                weeklyRate <= 0 ||
                weeklyRate >= 10000 || 
                Math.Round(weeklyRate, 2) != weeklyRate) 
            {
                ShowMessage(FindResource("ValidationError").ToString(),
                    FindResource("WeeklyRateInvalid").ToString(),
                    MessageType.Warning);
                WeeklyRateTextBox.Focus();
                return false;
            }


            return true;
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