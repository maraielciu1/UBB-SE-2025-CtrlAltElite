﻿using MarketPlace924.Domain;
using MarketPlace924.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MarketPlace924.Service
{
    public class UserService
    {
        private UserRepository _userRepository;
        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> RegisterUser(string username, string password, string email, string phoneNumber, int role)
        {
            if (!UserValidator.ValidateUsername(username))
            {
                throw new Exception("Username must be at least 4 characters long.");
            }
            if (!UserValidator.ValidateEmail(email))
            {
                throw new Exception("Invalid email address format.");
            }
            if (!UserValidator.ValidatePassword(password))
            {
                throw new Exception("The password must be at least 8 characters long, have at least 1 uppercase letter, at least 1 digit and at least 1 special character.");
            }
            if (!UserValidator.ValidatePhone(phoneNumber))
            {
                throw new Exception("The phone number should start with +40 area code followed by 9 digits.");
            }
            if (role != (int)UserRole.Buyer && role != (int)UserRole.Seller)
            {
                throw new Exception("Please select an account type (Buyer or Seller).");
            }
            if (await _userRepository.UsernameExists(username))
            {
                throw new Exception("Username already exists.");
            }
            if (await _userRepository.EmailExists(email))
            {
                throw new Exception("Email is already in use.");
            }

            var hashedPassword = HashPassword(password);

            var userRole = (UserRole)role;
            var newUser = new User(0, username, email, phoneNumber, hashedPassword, userRole, 0, null, false);

            await _userRepository.AddUser(newUser);

            return true;
        }

        public bool CheckEmailInLogIn(string email)
        {
            return UserValidator.ValidateEmail(email);
        }

        public static User GetUser(string username)
        {
            return new User();
        }

        public async Task<bool> CanUserLogin(string email, string password)
        {

			if (await _userRepository.EmailExists(email))
			{
				var user = await GetUserByEmail(email);
				if (user == null)
				{
					return false;
				}
				
                if (user.Password.StartsWith("plain:"))
                {
                    return user.Password == "plain:" + password;
                }
				return user.Password == HashPassword(password);
			}

            return false;
        }

        public async Task UpdateUserFailedLogins(User user, int newValueOfFailedLogIns)
        {
            await _userRepository.UpdateUserFailedLoginsCount(user, newValueOfFailedLogIns);
        }

        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
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
            return await _userRepository.GetFailedLoginsCountByUserId(userId);
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

            if (user is null) throw new ArgumentNullException($"{email} is not a user");

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
                var remainingTime = (user?.BannedUntil ?? DateTime.Now) - DateTime.Now;
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

        public static bool VerifyCaptcha(string enteredCaptcha, string generatedCaptcha)
        {
            return enteredCaptcha == generatedCaptcha;
        }


        public async Task<(bool Success, string Message, User? User)> LoginAsync(string email, string password, string enteredCaptcha, string generatedCaptcha)
        {
            if (string.IsNullOrEmpty(email))
                return (false, "Email cannot be empty!", null);

            if (string.IsNullOrEmpty(password))
                return (false, "Password cannot be empty!", null);

            if (!CheckEmailInLogIn(email))
                return (false, "Email does not have the right format!", null);

            if (!VerifyCaptcha(enteredCaptcha, generatedCaptcha))
                return (false, "Captcha verification failed.", null);

            if (!await IsUser(email))
                return (false, "Email does not exist.", null);

            var user = await GetUserByEmail(email);
            if(user == null)
                return (false, "Email does not exist.", null);
            if(user.IsBanned)
                return (false, "User is banned.", null);
            return (true, "Success", user);
        }
		public async Task<List<User>> GetAll()
		{
			return await _userRepository.GetAll();
		}
	}
}
