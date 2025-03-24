using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using MarketPlace924.Service;
using MarketPlace924.Domain;
using System.Threading.Tasks;

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
			InitializeComponent();
			_captchaService = new CaptchaService();

			GenerateCaptcha();
		}

		private async void LoginButton_Click(object sender, RoutedEventArgs e)
		{
			ErrorMessage.Text = string.Empty;
			var email = UserEmailBox.Text;
			var password = PasswordTextBox.Password;
			var enteredCapthca = CaptchaEnteredCode.Text;

			ValidateInputAsync(email, password, enteredCapthca);

			var user = await _userService.GetUserByEmail(email);

			if (await _userService.IsSuspended(email))
			{
				ErrorMessage.Text = "Too many failed attempts. Please try again later.";
				FailedLoginAttemptsText.Visibility = Visibility.Visible;
				FailedLoginAttemptsText.Text = $"Failed Login Attempts: {user.FailedLogins}";
				return;
			}

			if (!await _userService.CanUserLogin(email, password))
			{
				var failedLoginCount = user.FailedLogins + 1;
				await _userService.UpdateUserFailedLogins(user, failedLoginCount);

				FailedLoginAttemptsText.Text = $"Failed Login Attempts: {failedLoginCount}";
				FailedLoginAttemptsText.Visibility = Visibility.Visible;

				if (failedLoginCount >= 5)
				{
					await _userService.SuspendUserForSeconds(email, 5);
					ErrorMessage.Text = "Too many failed attempts. Please try again later.";
					_banEndTime = DateTime.Now.AddSeconds(5);
					StartBanCountdown(user);
				}
				else
				{
					ErrorMessage.Text = "Login failed";
				}

				return;
			}

			ErrorMessage.Text = "Login successful";
			await _userService.UpdateUserFailedLogins(user, 0);
			FailedLoginAttemptsText.Visibility = Visibility.Collapsed;
		}

		private async Task ValidateInputAsync(string email, string password, string enteredCapthca)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(enteredCapthca))
            {
                ErrorMessage.Text = "Please fill in all fields";
                return;
            }

            if (enteredCapthca != _generatedCaptcha)
            {
                ErrorMessage.Text = "Captcha is incorrect";
                GenerateCaptcha();
                return;
            }

            if (await _userService.IsUser(email))
            {
                return;
            }
            ErrorMessage.Text = "Email does not exist";
            return;
        }

        private void StartBanCountdown(User user)
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
					ErrorMessage.Text = string.Empty;
					FailedLoginAttemptsText.Text = "Failed Login Attempts: 0";

					if (user is null)
						return;

					_userService.UpdateUserFailedLogins(user, 0);
				}
				else
				{
					ErrorMessage.Text = $"Too many failed attempts. Try again in {remainingTime.Seconds}s";
				}
			};
			_banTimer.Start();
		}

		private void GenerateCaptcha()
		{
			CaptchaText.Text = _captchaService.GenerateCaptcha();
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
