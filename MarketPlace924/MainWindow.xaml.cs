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
using MarketPlace924.Repository;
using MarketPlace924.Service;
using MarketPlace924.View;
using MarketPlace924.DBConnection;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MarketPlace924
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            // Initialize Database Connection and Services
            var dbConnection = new DBConnection.DBConnection(); // Using your DBConnection class
            var userRepo = new UserRepository(dbConnection);
            var userService = new UserService(userRepo);

            // Create a Frame and navigate to LoginView, passing the UserService
            Frame rootFrame = new Frame();
            rootFrame.Navigate(typeof(LoginView), userService);

            // Set the content of the window
            this.Content = rootFrame;
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            myButton.Content = "Clicked";
        }
    }
}
