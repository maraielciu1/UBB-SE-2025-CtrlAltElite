using MarketPlace924.Domain;
using MarketPlace924.Service;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MarketPlace924.ViewModel
{
	public class AdminViewModel
	{
		public ObservableCollection<UserRowViewModel> Users { get; set; }
		public ICommand BanCommand { get; }

		private readonly AdminService _adminService;

		private readonly UserService _userService;

		public AdminViewModel(AdminService adminService, UserService userService)
		{
			_userService = userService;

			var users = new[]
			{
				new User { Username = "john_doe", Role = UserRole.Seller },
				new User { Username = "jane_smith", Role = UserRole.Buyer }
			};

			Users = new ObservableCollection<UserRowViewModel>(
				users.Select(user => new UserRowViewModel(user)));

			BanCommand = new RelayCommand(async (user) => await BanUser(user as User));
			_adminService = adminService;
		}

		public async Task BanUser(User user)
		{
			if (user != null)
			{
				Users.Remove(Users.Where(u => u.Username != user.Username).First());
				ShowBanDialog(user.Username);
			}
		}

		private async void ShowBanDialog(string username)
		{
			var dialog = new ContentDialog
			{
				Title = "User Banned",
				Content = $"{username} has been banned.",
				CloseButtonText = "OK"
			};
			dialog.ShowAsync();
		}
	}
}
