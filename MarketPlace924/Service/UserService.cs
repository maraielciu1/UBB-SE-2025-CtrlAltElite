using MarketPlace924.Domain;
using MarketPlace924.Repository;
using System;
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
			if (_userRepository.UsernameExists(email))
			{
				var user = await GetUserByEmail(email);

				return user?.Password == HashPassowrd(password);
			}

			return false;
		}

		public void UpdateUserFailedLogins(User user, int NewValueOfFailedLogIns)
		{
			_userRepository.UpdateUserFailedLoginsCount(user, NewValueOfFailedLogIns);
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
			return _userRepository.GetFailedLoginsCountByUserId(userId);
		}

		public bool IsUser(string email)
		{
			return _userRepository.UsernameExists(email);
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
			_userRepository.UpdateUser(user);
		}

	}
}
