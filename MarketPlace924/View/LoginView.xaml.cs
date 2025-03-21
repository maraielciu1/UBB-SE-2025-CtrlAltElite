using System;
using MarketPlace924.Service;
using MarketPlace924.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MarketPlace924.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginView : Page
    {
        public LoginViewModel ViewModel { get; set; }
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

            string enteredCapthca = CaptchaEnteredCode.Text;


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

            if (!ViewModel.UserService.checkExistanceOfEmail(email))
            {
                ErrorMessage.Text = "Email does not exist";
                return;
            }

            var user = ViewModel.UserService.getUserByEmail(email); // Fetch user only once

            // **Check if the user is banned**
            if (!ViewModel.UserService.canUserLogInNow(email)) // Corrected condition
            {
                ErrorMessage.Text = "Too many failed attempts. Please try again later.";
                FailedLoginAttemptsText.Visibility = Visibility.Visible;
                FailedLoginAttemptsText.Text = $"Failed Login Attempts: {user.FailedLogIns}";
                return;
            }

            // **Check Login Credentials**
            if (ViewModel.UserService.validateLogin(email, password))
            {
                ErrorMessage.Text = "Login successful";
                ViewModel.UserService.UpdateUserFailedLogins(user, 0); // Reset failed attempts
                FailedLoginAttemptsText.Visibility = Visibility.Collapsed;
                ViewModel.LoginSuccessCallback.OnLoginSuccess(user);
            }
            else
            {
                int newFailedAttempts = user.FailedLogIns + 1;
                ViewModel.UserService.UpdateUserFailedLogins(user, newFailedAttempts);

                FailedLoginAttemptsText.Text = $"Failed Login Attempts: {newFailedAttempts}";
                FailedLoginAttemptsText.Visibility = Visibility.Visible;

                if (newFailedAttempts >= 5) // **Ban user if they fail 5 times**
                {
                    ViewModel.UserService.BanUserTemporary(email, 5); // Ban for 5 seconds
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
                    var user = ViewModel.UserService.getUserByEmail(UserEmailBox.Text);
                    ViewModel.UserService.UpdateUserFailedLogins(user, 0);

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
            if (e.Parameter is LoginViewModel loginViewNavigateParams)
            {
                ViewModel = loginViewNavigateParams;
            }
        }
    }
}