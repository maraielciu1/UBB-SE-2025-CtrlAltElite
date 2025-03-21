using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MarketPlace924.Service;
using MarketPlace924.View;
using Microsoft.UI.Xaml;

public class LoginViewModel : INotifyPropertyChanged
{

    public readonly UserService _userService;
    private readonly OnLoginSuccessCallback _successCallback;
    private readonly CaptchaService _captchaService;
    private string _email;
    private string _password;
    private string _errorMessage;
    private int _failedAttempts;
    private bool _isLoginEnabled = true;
    private string _captchaText;
    private string _captchaEnteredCode;
    private DispatcherTimer _banTimer;
    private DateTime _banEndTime;

    public Action? NavigateToSignUp { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public string Email
    {
        get => _email;
        set
        {
            _email = value;
            OnPropertyChanged(nameof(Email));
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

    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            _errorMessage = value;
            OnPropertyChanged(nameof(ErrorMessage));
        }
    }

    public string FailedAttemptsText => $"Failed Login Attempts: {_failedAttempts}";

    public bool IsLoginEnabled
    {
        get => _isLoginEnabled;
        set
        {
            _isLoginEnabled = value;
            OnPropertyChanged(nameof(IsLoginEnabled));
        }
    }

    public string CaptchaText
    {
        get => _captchaText;
        set
        {
            _captchaText = value;
            OnPropertyChanged(nameof(CaptchaText));
        }
    }

    public string CaptchaEnteredCode
    {
        get => _captchaEnteredCode;
        set
        {
            _captchaEnteredCode = value;
            OnPropertyChanged(nameof(CaptchaEnteredCode));
        }
    }

    public ICommand LoginCommand { get; }
    

    public LoginViewModel(UserService userService, OnLoginSuccessCallback successCallback)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _successCallback = successCallback ?? throw new ArgumentNullException(nameof(successCallback));
        _captchaService = new CaptchaService();

        GenerateCaptcha();

        LoginCommand = new RelayCommand(async () => await ExecuteLogin());
    }

    private async Task ExecuteLogin()
    {
        ErrorMessage = "";

        if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(CaptchaEnteredCode))
        {
            ErrorMessage = "Please fill in all fields.";
            return;
        }

        if (!UserService.VerifyCaptcha(CaptchaEnteredCode,CaptchaText))
        {
            ErrorMessage = "Captcha verification failed.";
            GenerateCaptcha();
            return;
        }
        if (!await _userService.IsUser(Email))
        {
            ErrorMessage = "Email does not exist.";
            return;
        }

        var user = await _userService.GetUserByEmail(Email);


        if (user == null)
        {
            ErrorMessage = "Email does not exist.";
            return;
        }

        if (await _userService.IsSuspended(Email))
        {
            TimeSpan remainingTime = _banEndTime - DateTime.Now;
            ErrorMessage = $"Too many failed attempts. Try again in {remainingTime.Seconds}s";
            return;
        }

        if (!await _userService.CanUserLogin(Email, Password))
        {
            _failedAttempts++;
            await _userService.UpdateUserFailedLogins(user, _failedAttempts);
            ErrorMessage = $"Login failed";

            if (_failedAttempts >= 5)
            {
                await _userService.SuspendUserForSeconds(Email, 5);
                _banEndTime = DateTime.Now.AddSeconds(5);
                StartBanTimer();
            }
        }
        else
        {

            ErrorMessage = "Login successful!";
            _failedAttempts = 0;
            await _userService.UpdateUserFailedLogins(user, 0);
            IsLoginEnabled = true;
            _successCallback.OnLoginSuccess(user);
        }
        OnPropertyChanged(nameof(FailedAttemptsText));
    }


   
    private void StartBanTimer()
    {
        IsLoginEnabled = false;
        ErrorMessage = "Too many failed attempts. Please wait...";

        _banTimer = new DispatcherTimer();
        _banTimer.Interval = TimeSpan.FromSeconds(1);
        _banTimer.Tick += async (s, e) =>
        {
            TimeSpan remainingTime = _banEndTime - DateTime.Now;

            if (remainingTime.TotalSeconds <= 0)
            {
                _banTimer.Stop();
                IsLoginEnabled = true;
                ErrorMessage = "";

                await _userService.ResetFailedLogins(Email);
                _failedAttempts = 0;  // Reset in UI
                OnPropertyChanged(nameof(FailedAttemptsText));
            }
            else
            {
                ErrorMessage = $"Too many failed attempts. Try again in {remainingTime.Seconds}s";
            }
        };
        _banTimer.Start();
    }


    private void GenerateCaptcha()
    {
        CaptchaText = CaptchaService.GenerateCaptcha();
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
