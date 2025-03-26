using MarketPlace924.Domain;
using System;
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

		public UserRowViewModel(User user)
		{
			User = user;
			BanUserCommand = new RelayCommand(async () => BanUser()); // Assuming RelayCommand handles ICommand logic
		}

		// Method to "ban" the user
		private async void BanUser()
		{
			// Perform your banning logic here, for example, removing from the list or updating the status
		}
	}
}
