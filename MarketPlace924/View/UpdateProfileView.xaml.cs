using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MarketPlace924.ViewModel;
using Microsoft.UI.Xaml;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace MarketPlace924.View
{
    public sealed partial class UpdateProfileView : Page
    {
        public UpdateProfileView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is SellerProfileViewModel viewModel)
            {
                DataContext = viewModel;
            }
        }

        private async void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            var viewModel = (SellerProfileViewModel)DataContext;
            List<string> validationErrors = viewModel.ValidateFields();

            if (validationErrors.Any())
            {
                string errorMessage = string.Join("\n", validationErrors);
                await ShowDialog("Validation Errors", errorMessage);
            }
            else
            {
                viewModel.UpdateProfileCommand.Execute(null);
            }
        }

        private async Task ShowDialog(string title, string message)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = App.m_window.Content.XamlRoot
            };

            await dialog.ShowAsync();
        }

        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
    }
}