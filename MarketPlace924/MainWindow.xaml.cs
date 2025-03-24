using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MarketPlace924.Repository;
using MarketPlace924.Service;
using MarketPlace924.View;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MarketPlace924
{
	/// <summary>
	/// An empty window that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainWindow : Window
    {
		public static MainWindow _instance { get; set; }
		public MainWindow()
        {
            InitializeComponent();
			_instance = this;

            
            var dbConnection = new DBConnection.DatabaseConnection(); // Using your DBConnection class
            var userRepository = new UserRepository(dbConnection);
            var userService = new UserService(userRepository);

            
            Frame rootFrame = new Frame();
            rootFrame.Navigate(typeof(LoginView), userService);

            
            Content = rootFrame;
        }

		public static void SetContent(Frame newFrame)
		{
			_instance.Content = newFrame;
		}
    }
}
