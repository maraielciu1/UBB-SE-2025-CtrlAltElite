using MarketPlace924.Domain;
using MarketPlace924.Repository;
using System;
using System.Threading.Tasks;

namespace MarketPlace924.Service
{
	public class AdminService
	{
		private readonly UserRepository _userRepository;

		public AdminService(UserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		public async Task BanUser(User user)
		{
			if (user.IsBanned)
			{
				user.BannedUntil = null;
				user.IsBanned = false;
			}
			else
			{
				user.BannedUntil = DateTime.Now.AddYears(10);
				user.IsBanned = true;
			}
			await _userRepository.UpdateUser(user);
		}

		internal async Task SetAdmin(User user)
		{
			user.Role = UserRole.Admin;
			await _userRepository.UpdateUser(user);
		}
	}
}
