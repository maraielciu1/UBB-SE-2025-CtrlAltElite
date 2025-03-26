using MarketPlace924.Service;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MarketPlace924.ViewModel
{
    class SignUpViewModel : INotifyPropertyChanged
    {
        private UserService _userService;
        public Action? NavigateToLogin { get; set; }

        public SignUpViewModel(UserService userService)
        {
            _userService = userService;
            SignupCommand = new RelayCommand(ExecuteSignup);
            _username = string.Empty;
            _email = string.Empty;
            _phoneNumber = string.Empty;
            _password = string.Empty;
        }

        private string _username;
        private string _email;
        private string _phoneNumber;
        private string _password;
        private int _role;

        public string Username { get => _username; set { _username = value; OnPropertyChanged(); } }
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }
        public string PhoneNumber { get => _phoneNumber; set { _phoneNumber = value; OnPropertyChanged(); } }
        public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }
        public int Role { get => _role; set { _role = value; OnPropertyChanged(); } }

        public ICommand SignupCommand { get; }

        private async void ExecuteSignup(object parameter)
        {
            try
            {
                await _userService.RegisterUser(Username, Password, Email, PhoneNumber, Role);
                await ShowDialog("Success", "Your account has been created successfully!");
                NavigateToLogin?.Invoke();
            }
            catch (Exception ex)
            {
                await ShowDialog("Error", ex.Message);
            }
        }

        private async System.Threading.Tasks.Task ShowDialog(string title, string message)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = App.m_window.Content.XamlRoot
            };

            await dialog.ShowAsync();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
