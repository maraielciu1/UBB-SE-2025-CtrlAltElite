using MarketPlace924.DBConnection;
using MarketPlace924.Domain;
using MarketPlace924.Repository;
using MarketPlace924.Service;
using MarketPlace924.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MarketPlace924.View
{
    public sealed partial class MyMarketView : Page
    {
        private MyMarketViewModel? _viewModel;

        // Gets the ViewModel associated with this page.
        public MyMarketViewModel ViewModel => _viewModel;
        public MyMarketView()
        {
            this.InitializeComponent();
        }


        // Called when the page is navigated to.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // If the navigation parameter contains a ViewModel, assign it to the _viewModel
            if (e.Parameter is MyMarketViewModel viewModel)
            {

                _viewModel = viewModel;
                DataContext = _viewModel;
            }

        }

        // Handles the text change event in the product search text box.
        private void OnSearchProductTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (_viewModel != null && textBox != null)
            {
                _viewModel.FilterProducts(textBox.Text); // Pass the search text to the ViewModel
            }
        }

        // Handles the text change event in the seller search text box.
        private void OnSearchSellerTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (_viewModel != null && textBox != null)
            {
                _viewModel.FilterFollowing(textBox.Text); // Pass the search text to the ViewModel
            }
        }

        // Navigates to the seller's profile page.
        private void OnPersonPictureTapped(object sender, RoutedEventArgs e)
        {
            // Get the tapped Seller object (the DataContext of the tapped item)
            var tappedSeller = (sender as FrameworkElement)?.DataContext as Seller;

            if (tappedSeller != null)
            {
                // Navigate to the Seller's profile page and pass the seller data
                Frame.Navigate(typeof(MyMarketProfileView), tappedSeller);
            }
        }
    }
}
