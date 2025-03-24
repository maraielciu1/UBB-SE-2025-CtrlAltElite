using MarketPlace924.Domain;
using MarketPlace924.Repository;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MarketPlace924.Service
{
	class UserService
	{
		private UserRepository _userRepository;
		public UserService(UserRepository userRepository)
		{
			_userRepository = userRepository;
		}
		public void addUser(string username, string password, string email, int role)
		{

		}

        public bool RegisterUser(string username, string password, string email, string phoneNumber, int role)
        {

            if (!UserValidator.validateUsername(username))
            {
                throw new Exception("Username must be at least 4 characters long.");
            }
            if (!UserValidator.validateEmail(email))
            {
                throw new Exception("Invalid email address format.");
            }
            if (!UserValidator.validatePassword(password))
            {
                throw new Exception("The password must be at least 8 characters long, have at least 1 uppercase letter, at least 1 digit and at least 1 special character.");
            }
            if (!UserValidator.validatePhone(phoneNumber))
            {
                throw new Exception("The phone number should start with +40 area code followed by 9 digits.");
            }
            if (_userRepository.UsernameExists(username))
            {
                throw new Exception("Username already exists.");
            }
            if (_userRepository.EmailExists(email))
            {
                throw new Exception("Email is already in use.");
            }

            string hashedPassword = HashPassword(password);
            Debug.WriteLine($"Role passed in: {role}");

            UserRole userRole = (UserRole)role;
            User newUser = new User(0, username, email, phoneNumber, hashedPassword, userRole, 0, null, false);
            Debug.WriteLine($"Parsed userRole: {userRole}");

            _userRepository.AddUser(newUser);

            return true;
        }

        public User GetUser(string username)
		{
			return new User();
		}

		public async Task<bool> CanUserLogin(string email, string password)
		{
			if (await _userRepository.EmailExists(email))
			{
				var user = await GetUserByEmail(email);

				return user?.Password == HashPassowrd(password);
			}

			return false;
		}

		public async Task UpdateUserFailedLogins(User user, int NewValueOfFailedLogIns)
		{
			await _userRepository.UpdateUserFailedLoginsCount(user, NewValueOfFailedLogIns);
		}

		public string HashPassowrd(string password)
		{
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

		public async Task<User?> GetUserByEmail(string email)
		{
			return await _userRepository.GetUserByEmail(email);
		}

		public async Task<int> GetFailedLoginsCountByEmail(string email)
		{
			var user = await GetUserByEmail(email);

			if (user is null) throw new ArgumentNullException($"{email} is not a user");

			int userId = user.UserId;
			return await  _userRepository.GetFailedLoginsCountByUserId(userId);
		}

		public async Task<bool> IsUser(string email)
		{
			return await _userRepository.EmailExists(email);
		}

		public async Task<bool> IsSuspended(string email)
		{
			var user = await GetUserByEmail(email);

			if (user is null) throw new ArgumentNullException($"{email} is not a user");

			if (user.BannedUntil.HasValue && user.BannedUntil > DateTime.Now)
			{
				return true;
			}

			return false;
		}


		public async Task SuspendUserForSeconds(string email, int seconds)
		{
			var user = await GetUserByEmail(email);

			if(user is null) throw new ArgumentNullException($"{email} is not a user");

			user.BannedUntil = DateTime.Now.AddSeconds(seconds);
			await _userRepository.UpdateUser(user);
		}

        public async Task<string> ValidateLogin(string email, string password, string enteredCaptcha, string generatedCaptcha)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(enteredCaptcha))
                return "Please fill in all fields.";

            if (enteredCaptcha != generatedCaptcha)
                return "Captcha verification failed.";

            if (!await IsUser(email))
                return "Email does not exist.";

            if (await IsSuspended(email))
            {
                var user = await GetUserByEmail(email);
                TimeSpan remainingTime = (user.BannedUntil ?? DateTime.Now) - DateTime.Now;
                return $"Too many failed attempts. Try again in {remainingTime.Seconds}s";
            }

            if (!await CanUserLogin(email, password))
                return "Login failed";

            return "Success"; 
        }
        public async Task HandleFailedLogin(string email)
        {
            var user = await GetUserByEmail(email);
            if (user == null) return;

            int failedAttempts = await GetFailedLoginsCountByEmail(email) + 1;
			Debug.WriteLine(failedAttempts);
            await UpdateUserFailedLogins(user, failedAttempts);

            if (failedAttempts >= 5) 
            {
                await SuspendUserForSeconds(email, 5);
            }
        }
        public async Task ResetFailedLogins(string email)
        {
            var user = await GetUserByEmail(email);
            if (user != null)
            {
                await UpdateUserFailedLogins(user, 0);
            }
        }

		public bool VerifyCaptcha(string enteredCaptcha, string generatedCaptcha)
		{
            return enteredCaptcha == generatedCaptcha;
        }






    }
}
