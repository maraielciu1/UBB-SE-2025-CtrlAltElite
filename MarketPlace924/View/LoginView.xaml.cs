using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MarketPlace924.Service;
using Microsoft.IdentityModel.Tokens;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MarketPlace924.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginView : Page
    {
        private UserService _userService;
        private DispatcherTimer _banTimer;
        private DateTime _banEndTime;
        private readonly CaptchaService _captchaService;
        private string _generatedCaptcha;

        public LoginView()
        {
            this.InitializeComponent();
            _captchaService = new CaptchaService();

            GenerateAndSetCaptcha();



        }
        private void GenerateAndSetCaptcha()
        {
            _generatedCaptcha = _captchaService.GenerateCaptcha();
            CaptchaText.Text = _generatedCaptcha;
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessage.Text = "";
            string email = UserEmailBox.Text;
            string password = PasswordTextBox.Password;

            string enteredCapthca=CaptchaEnteredCode.Text;



            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(enteredCapthca))
            {
                ErrorMessage.Text = "Please fill in all fields";
                return;
            }


            if (enteredCapthca != _generatedCaptcha)
            {
                ErrorMessage.Text = "Captcha is incorrect";
                GenerateAndSetCaptcha();
                return;
            }
            if (!_userService.checkExistanceOfEmail(email))
            {
                ErrorMessage.Text = "Email does not exist";
                return;
            }

            var user = _userService.getUserByEmail(email); // Fetch user only once

            // **Check if the user is banned**
            if (!_userService.canUserLogInNow(email)) // Corrected condition
            {
                ErrorMessage.Text = "Too many failed attempts. Please try again later.";
                FailedLoginAttemptsText.Visibility = Visibility.Visible;
                FailedLoginAttemptsText.Text = $"Failed Login Attempts: {user.FailedLogIns}";
                return;
            }

            // **Check Login Credentials**
            if (_userService.validateLogin(email, password))
            {
                ErrorMessage.Text = "Login successful";
                _userService.UpdateUserFailedLogins(user, 0); // Reset failed attempts
                FailedLoginAttemptsText.Visibility = Visibility.Collapsed;
            }
            else
            {
                int newFailedAttempts = user.FailedLogIns + 1;
                _userService.UpdateUserFailedLogins(user, newFailedAttempts);

                FailedLoginAttemptsText.Text = $"Failed Login Attempts: {newFailedAttempts}";
                FailedLoginAttemptsText.Visibility = Visibility.Visible;

                if (newFailedAttempts >= 5) // **Ban user if they fail 5 times**
                {
                    _userService.BanUserTemporary(email, 5); // Ban for 5 seconds
                    ErrorMessage.Text = "Too many failed attempts. Please try again later.";
                    _banEndTime = DateTime.Now.AddSeconds(5);
                    StartBanCountdown();
                }
                else
                {
                    ErrorMessage.Text = "Login failed";
                }
            }
        }

        private void StartBanCountdown()
        {
            LoginButton.IsEnabled = false;
            ErrorMessage.Text = "Too many failed attempts. Please wait...";

            _banTimer = new DispatcherTimer();
            _banTimer.Interval = TimeSpan.FromSeconds(1);
            _banTimer.Tick += (s, e) =>
            {
                TimeSpan remainingTime = _banEndTime - DateTime.Now;
                if (remainingTime.TotalSeconds <= 0)
                {
                    _banTimer.Stop();
                    LoginButton.IsEnabled = true;
                    ErrorMessage.Text = "";
                    FailedLoginAttemptsText.Text = "Failed Login Attempts: 0"; 
                    var user = _userService.getUserByEmail(UserEmailBox.Text);
                    _userService.UpdateUserFailedLogins(user, 0);

                    FailedLoginAttemptsText.Text = "Failed Login Attempts: 0";


                }
                else
                {
                    ErrorMessage.Text = $"Too many failed attempts. Try again in {remainingTime.Seconds}s";
                }
            };
            _banTimer.Start();
        }



        private void RegisterButtonTextBlock_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ErrorMessage.Text = "";

        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is UserService userService)
            {
                _userService = userService;
            }
            
        }
    }
}
