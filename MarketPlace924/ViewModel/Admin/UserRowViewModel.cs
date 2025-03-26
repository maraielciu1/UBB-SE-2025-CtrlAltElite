using MarketPlace924.Domain;
using System.Windows.Input;

namespace MarketPlace924.ViewModel
{
	public class UserRowViewModel
	{
		public string Username { get; set; }
		public UserRole Role { get; set; }

		// Command to ban the user
		public ICommand BanUserCommand { get; }

		public UserRowViewModel(User user)
		{
			Username = user.Username;
			Role = user.Role;
			BanUserCommand = new RelayCommand(async () => BanUser()); // Assuming RelayCommand handles ICommand logic
		}

		// Method to "ban" the user
		private async void BanUser()
		{
			// Perform your banning logic here, for example, removing from the list or updating the status
		}
	}
}
