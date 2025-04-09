using RentACar.Model;
using RentACar.Services;
using RentACar.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RentACar.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly Window _window;
        private string _username;
        private string _password;
        private bool _isPasswordVisible;
        private readonly rentacarContext _dbContext;

        public LoginViewModel(Window window)
        {
            _window = window;
            LoginCommand = new RelayCommand(ExecuteLogin);
            MinimizeCommand = new RelayCommand(ExecuteMinimize);
            CloseCommand = new RelayCommand(ExecuteClose);
            TogglePasswordVisibilityCommand = new RelayCommand(ExecuteTogglePasswordVisibility);
            _dbContext = new rentacarContext();
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public bool IsPasswordVisible
        {
            get => _isPasswordVisible;
            set
            {
                _isPasswordVisible = value;
                OnPropertyChanged(nameof(IsPasswordVisible));
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand MinimizeCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand TogglePasswordVisibilityCommand { get; }

        private void ExecuteLogin(object parameter)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    var messageBox = new CustomMessageBox(
                        Application.Current.TryFindResource("LoginError") as string ?? "Login Error",
                        Application.Current.TryFindResource("PleaseEnterCredentials") as string ?? "Please enter username and password.",
                        MessageType.Warning);
                    messageBox.ShowDialog();
                    return;
                }

                var user = _dbContext.Korisniks.FirstOrDefault(
                    u => u.KorisnickoIme == Username && u.Lozinka == Password);

                if (user == null)
                {
                    var messageBox = new CustomMessageBox(
                        Application.Current.TryFindResource("LoginError") as string ?? "Login Error",
                        Application.Current.TryFindResource("InvalidCredentials") as string ?? "Invalid username or password.",
                        MessageType.Error);
                    messageBox.ShowDialog();
                    return;
                }

                var loginWindow = _window as LoginWindow;
                int currentThemeIndex = loginWindow?.currentThemeIndex ?? 0;
                int currentLanguageIndex = loginWindow?.currentLanguageIndex ?? 0;

                bool dbChanges = false;
                if (user.Tema == null)
                {
                    user.Tema = currentThemeIndex;
                    dbChanges = true;
                }

                if (user.Jezik == null)
                {
                    user.Jezik = currentLanguageIndex;
                    dbChanges = true;
                }

                if (dbChanges)
                {
                    _dbContext.SaveChanges();
                }

                //LocalizationService.ChangeLanguage((LocalizationService.Language)user.Jezik);
                //ThemeService.ChangeTheme((ThemeService.ThemeType)user.Tema);

                if (user.TipNaloga.Equals("Administrator", StringComparison.OrdinalIgnoreCase))
                {
                    MainWindow mainWindow = new MainWindow(user);
                    mainWindow.Show();
                    _window.Close();
                }
                else if (user.TipNaloga.Equals("Agent", StringComparison.OrdinalIgnoreCase))
                {

                    MainAgentWindow mainAgentWindow = new MainAgentWindow(user);
                    mainAgentWindow.Show();
                    _window.Close();

                }
                else
                {
                    var messageBox = new CustomMessageBox(
                        Application.Current.TryFindResource("LoginError") as string ?? "Login Error",
                        Application.Current.TryFindResource("UnsupportedAccountType") as string ?? "Unsupported account type.",
                        MessageType.Warning);
                    messageBox.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                var messageBox = new CustomMessageBox(
                    Application.Current.TryFindResource("Error") as string ?? "Error",
                    $"{Application.Current.TryFindResource("LoginFailure") as string ?? "Login failed"}: {ex.Message}",
                    MessageType.Error);
                messageBox.ShowDialog();
            }
        }

        private void ExecuteMinimize(object parameter)
        {
            _window.WindowState = WindowState.Minimized;
        }

        private void ExecuteClose(object parameter)
        {
            _window.Close();
        }

        private void ExecuteTogglePasswordVisibility(object parameter)
        {
            IsPasswordVisible = !IsPasswordVisible;
        }
    }
}