using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;
using MarketPlace924.Domain;
using Microsoft.UI.Xaml.Navigation;
using MarketPlace924.ViewModel;
using MarketPlace924.ViewModel.Admin;

namespace MarketPlace924.View.Admin
{
	public sealed partial class AdminView : Page
	{
		public ObservableCollection<User> Users { get; set; }

		public AdminView()
		{
			this.InitializeComponent();

			Users = new ObservableCollection<User>();
		}


		public AdminViewModel ViewModel
		{
			get => (AdminViewModel)DataContext;
			set => DataContext = value;
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			if (e.Parameter is AdminViewModel viewModel)
			{
				ViewModel = viewModel;

				UsersListView.ItemsSource = ViewModel.Users;
			}
		}
	}
}
