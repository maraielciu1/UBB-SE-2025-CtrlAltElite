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
			return password;
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

            return "Success"; // Login is valid
        }
        public async Task HandleFailedLogin(string email)
        {
            var user = await GetUserByEmail(email);
            if (user == null) return;

            int failedAttempts = await GetFailedLoginsCountByEmail(email) + 1;
			Debug.WriteLine(failedAttempts);
            await UpdateUserFailedLogins(user, failedAttempts);

            if (failedAttempts >= 5) // Ban user if 5 failed attempts
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
