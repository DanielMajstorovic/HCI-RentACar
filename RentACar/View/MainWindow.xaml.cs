using RentACar.Converters;
using RentACar.Services;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using RentACar.ViewModel;
using RentACar.Model;
using System.Linq;

namespace RentACar.View
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private bool _isMenuExpanded = true;
        public bool IsMenuExpanded
        {
            get => _isMenuExpanded;
            set
            {
                if (_isMenuExpanded != value)
                {
                    _isMenuExpanded = value;
                    OnPropertyChanged(nameof(IsMenuExpanded));
                }
            }
        }

        
        public int currentLanguageIndex = 0;
        public int currentThemeIndex = 0;

        
        private readonly string[] languageCodes = { "EN", "SR", "DE" };
        private readonly string[] themeNames = { "light", "dark", "sea" };

        
        private Button[] languageButtons;
        private Button[] themeButtons;

        
        private ICommand _minimizeCommand;
        public ICommand MinimizeCommand => _minimizeCommand ?? (_minimizeCommand = new RelayCommand(_ => WindowState = WindowState.Minimized));

        private ICommand _closeCommand;
        public ICommand CloseCommand => _closeCommand ?? (_closeCommand = new RelayCommand(_ => Close()));

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

        public MainWindow(Korisnik user)
        {
            InitializeComponent();
            DataContext = this;
            _currentUser = user;
            CurrentUser = user;

            
            if (_currentUser.Tema.HasValue)
            {
                currentThemeIndex = _currentUser.Tema.Value;
                ThemeService.ChangeTheme((ThemeService.ThemeType)currentThemeIndex);
            }

            
            if (_currentUser.Jezik.HasValue)
            {
                currentLanguageIndex = _currentUser.Jezik.Value;
                LocalizationService.ChangeLanguage((LocalizationService.Language)currentLanguageIndex);
            }

            
            DataContext = this;

            
            UpdateLanguageCode();
            UpdateThemeName();

            
            languagePopup.Opened += LanguagePopup_Opened;
            themePopup.Opened += ThemePopup_Opened;
        }

        private void LanguagePopup_Opened(object sender, EventArgs e)
        {
            
            if (languageButtons == null)
            {
                
                var popupContent = ((Border)languagePopup.Child).Child as StackPanel;
                if (popupContent != null)
                {
                    languageButtons = popupContent.Children.OfType<Button>().ToArray();
                }
            }

            
            UpdateLanguageButtonStyles();
        }

        private void ThemePopup_Opened(object sender, EventArgs e)
        {
            
            if (themeButtons == null)
            {
                
                var popupContent = ((Border)themePopup.Child).Child as StackPanel;
                if (popupContent != null)
                {
                    themeButtons = popupContent.Children.OfType<Button>().ToArray();
                }
            }

            
            UpdateThemeButtonStyles();
        }

        private void UpdateLanguageButtonStyles()
        {
            if (languageButtons == null)
                return;

            
            for (int i = 0; i < languageButtons.Length; i++)
            {
                if (i == currentLanguageIndex)
                {
                    
                    languageButtons[i].Background = FindResource("InputBackgroundBrush") as Brush;
                    languageButtons[i].FontWeight = FontWeights.Bold;
                }
                else
                {
                    
                    languageButtons[i].Background = Brushes.Transparent;
                    languageButtons[i].FontWeight = FontWeights.Normal;
                }
            }
        }

        private void UpdateThemeButtonStyles()
        {
            if (themeButtons == null)
                return;

            
            for (int i = 0; i < themeButtons.Length; i++)
            {
                if (i == currentThemeIndex)
                {
                    
                    themeButtons[i].Background = FindResource("InputBackgroundBrush") as Brush;
                    themeButtons[i].FontWeight = FontWeights.Bold;
                }
                else
                {
                    
                    themeButtons[i].Background = Brushes.Transparent;
                    themeButtons[i].FontWeight = FontWeights.Normal;
                }
            }
        }

        
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        
        private void OnLanguageClick(object sender, RoutedEventArgs e)
        {
            
            languagePopup.IsOpen = !languagePopup.IsOpen;
            
            themePopup.IsOpen = false;
        }

        
        private void OnThemeClick(object sender, RoutedEventArgs e)
        {
            
            themePopup.IsOpen = !themePopup.IsOpen;
            
            languagePopup.IsOpen = false;
        }

        
        private void OnLanguageItemClick(object sender, RoutedEventArgs e)
        {
            
            if (sender is Button button && button.Tag is string tagValue)
            {
                if (int.TryParse(tagValue, out int languageIndex))
                {

                    currentLanguageIndex = languageIndex;
                    var language = (LocalizationService.Language)currentLanguageIndex;
                    LocalizationService.ChangeLanguage(language);

                    using (var rc = new rentacarContext())
                    {
                        var user = rc.Korisniks.FirstOrDefault(u => u.IdKorisnik == _currentUser.IdKorisnik);
                        if (user != null)
                        {
                            user.Jezik = currentLanguageIndex;
                            rc.SaveChanges();
                        }
                    }

                    
                    UpdateLanguageCode();

                    
                    UpdateLanguageButtonStyles();

                    
                    languagePopup.IsOpen = false;
                }
            }
        }

        
        private void OnThemeItemClick(object sender, RoutedEventArgs e)
        {
            
            if (sender is Button button && button.Tag is string tagValue)
            {
                if (int.TryParse(tagValue, out int themeIndex))
                {
                    currentThemeIndex = themeIndex;
                    var theme = (ThemeService.ThemeType)currentThemeIndex;
                    ThemeService.ChangeTheme(theme);

                    using (var rc = new rentacarContext())
                    {
                        var user = rc.Korisniks.FirstOrDefault(u => u.IdKorisnik == _currentUser.IdKorisnik);
                        if (user != null)
                        {
                            user.Tema = currentThemeIndex;
                            rc.SaveChanges();
                        }
                    }

                    
                    UpdateThemeName();

                    
                    UpdateThemeButtonStyles();

                    
                    themePopup.IsOpen = false;
                }
            }
        }

        private void UpdateLanguageCode()
        {
            
            txtLanguageCode.Text = languageCodes[currentLanguageIndex];
        }

        private void UpdateThemeName()
        {
            
            txtThemeName.Text = themeNames[currentThemeIndex];
        }

        
        private void LogoutUser(object sender, RoutedEventArgs e)
        {
            
            var confirmLogout = new CustomMessageBox(
    (string)Application.Current.FindResource("LogoutConfirmation"),
    (string)Application.Current.FindResource("LogoutConfirmationMessage"),
    MessageType.Question,
    true);
            if (confirmLogout.ShowDialog() == true)
            {
                
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                
                Close();
            }
        }

        
        private void ToggleMenuButton_Click(object sender, RoutedEventArgs e)
        {
            IsMenuExpanded = !IsMenuExpanded;
        }

        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
           => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}