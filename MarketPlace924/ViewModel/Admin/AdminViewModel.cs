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
using System.Runtime.CompilerServices;

namespace MarketPlace924.ViewModel.Admin
{
	public class AdminViewModel
	{

		private ObservableCollection<UserRowViewModel> _items;

		public static AdminViewModel _instance;

		private List<User> users = new();
		public ObservableCollection<UserRowViewModel> Users
		{
			get
			{
				users = _userService.GetAll().Result;

				if (_items == null)
					_items = new ObservableCollection<UserRowViewModel>(users.Select(user => new UserRowViewModel(user, _adminService)));

				return _items;
			}
		}

		public ICommand BanCommand { get; }

		private readonly AdminService _adminService;

		private readonly UserService _userService;

		private readonly AnalyticsService _analyticsService;

		public int TotalUsersCount = 0;
		public List<ISeries> PieSeries { get; set; }

		public AdminViewModel(AdminService adminService, AnalyticsService analyticsService, UserService userService)
		{
			_instance = this;
			BanCommand = new RelayCommand(async (user) => await BanUser(user as User));
			_adminService = adminService;
			_analyticsService = analyticsService;

			SetupPieChart();
			_userService = userService;
		}

		private void SetupPieChart()
		{
			TotalUsersCount = _analyticsService.GetTotalUsersCount().Result;
			var buyersCount = _analyticsService.GetTotalBuyersCount().Result;

			PieSeries = new List<ISeries>
			{
				new PieSeries<double>
				{
					Values = new List<double> { buyersCount },
					Name = "Buyers",
					Fill = new SolidColorPaint(SKColors.LightYellow)
				},
				new PieSeries<double>
				{
					Values = new List<double> { TotalUsersCount - buyersCount },
					Name = "Sellers",
					Fill = new SolidColorPaint(SKColors.LightGreen)
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

		public async Task RefreshUsers()
		{
			users = await _userService.GetAll();
			_items.Clear();
			foreach(var user in users)
			{
				_items.Add(new UserRowViewModel(user, _adminService));
			}
		}
	}
}
