using MarketPlace924.Domain;
using MarketPlace924.Service;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MarketPlace924.ViewModel.Admin
{
	public class UserRowViewModel
	{
		public User User { get; }
		public int UserId => User.UserId;
		public string Username => User.Username;
		public string Email => User.Email;
		public string PhoneNumber => User.PhoneNumber;
		public string Role => User.Role.ToString();
		public int FailedLogins => User.FailedLogins;
		public DateTime? BannedUntil => User.BannedUntil;
		public bool IsBanned => User.IsBanned;

		// Command to ban the user
		public ICommand BanUserCommand { get; }
		public ICommand SetAdminCommand { get; }
		private readonly AdminService _adminService;

		public UserRowViewModel(User user, AdminService adminService)
		{
			User = user;
			BanUserCommand = new RelayCommand(async () => await BanUser()); // Assuming RelayCommand handles ICommand logic
			SetAdminCommand = new RelayCommand(async () => await SetAdmin()); // Assuming RelayCommand handles ICommand logic
			_adminService = adminService;
		}

		// Method to "ban" the user
		private async Task BanUser()
		{
			await _adminService.BanUser(User);
			await AdminViewModel._instance.RefreshUsers();
		}

		private async Task SetAdmin()
		{
			await _adminService.SetAdmin(User);
			await AdminViewModel._instance.RefreshUsers();

		}
	}
}
