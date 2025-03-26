using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using MarketPlace924.Domain;
using MarketPlace924.Service;
using Microsoft.UI.Xaml.Controls;
using SkiaSharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MarketPlace924.ViewModel.Admin
{
	public class AdminViewModel
	{
		private List<User> users = new()
			{
				new User { Username = "john_doe", Role = UserRole.Seller },
				new User { Username = "jane_smith", Role = UserRole.Buyer }
			};

		private ObservableCollection<UserRowViewModel> _items;

		public ObservableCollection<UserRowViewModel> Users
		{
			get
			{
				users = _userService.GetAll().Result;

				if (_items == null)
					_items = new ObservableCollection<UserRowViewModel>(users.Select(user => new UserRowViewModel(user)));

				return _items;
			}
		}

		public ICommand BanCommand { get; }

		private readonly AdminService _adminService;

		private readonly UserService _userService;

		private readonly BuyerService _buyerService;

		public List<ISeries> PieSeries { get; set; }

		public AdminViewModel(AdminService adminService, UserService userService)
		{
			_userService = userService;

			BanCommand = new RelayCommand(async (user) => await BanUser(user as User));
			_adminService = adminService;
			SetupPieChart();
		}

		private void SetupPieChart()
		{
			PieSeries = new List<ISeries>
			{
				new PieSeries<double>
				{
					Values = new List<double> { 60 },
					Name = "Buyers",
					Fill = new SolidColorPaint(SKColors.LightGreen)
				},
				new PieSeries<double>
				{
					Values = new List<double> { 40 }, // Second slice (40%)
					Name = "Sellers",
					Fill = new SolidColorPaint(SKColors.LightCoral)
				}
			};
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
