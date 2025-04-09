using RentACar.Services;
using RentACar.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
namespace RentACar.View
{
    
    public partial class LoginWindow : Window
    {
        public int currentLanguageIndex = 0;
        public int currentThemeIndex = 0;
        private LoginViewModel _viewModel;
        private readonly string[] languageCodes = { "EN", "SR", "DE" };
        private readonly string[] themeNames = { "light", "dark", "sea" };
        private Button[] languageButtons;
        private Button[] themeButtons;

        public LoginWindow()
        {
            InitializeComponent();
            _viewModel = new LoginViewModel(this);
            DataContext = _viewModel;
            ThemeService.ChangeTheme(ThemeService.ThemeType.Light);
            LocalizationService.ChangeLanguage(LocalizationService.Language.English);

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
            {
                DragMove();
            }
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

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.Password = txtPassword.Password;
            }
        }
    }
}