using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MarketPlace924.ViewModel.Admin;
using MarketPlace924.Domain;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MarketPlace924.View.Admin
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class UserRowView : UserControl
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


		public UserRowViewModel ViewModel
		{
			get => (UserRowViewModel)DataContext;
			set => DataContext = value;
		}

		public UserRowView(User user)
		{
			this.InitializeComponent();
			User = user;
		}
	}
}
