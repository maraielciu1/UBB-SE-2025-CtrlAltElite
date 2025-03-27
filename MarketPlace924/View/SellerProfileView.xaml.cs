using MarketPlace924.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace MarketPlace924.View;

public sealed partial class SellerProfileView : Page
{
    private SellerProfileViewModel? _viewModel;

    public SellerProfileViewModel? ViewModel => _viewModel;

    public SellerProfileView()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is SellerProfileViewModel viewModel)
        {
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        var viewModel = DataContext as SellerProfileViewModel;
        if (viewModel != null && textBox != null)
        {
            viewModel.FilterProducts(textBox.Text);
        }
    }

    private void OnUpdateProfileButtonClick(object sender, RoutedEventArgs e)
    {
        var viewModel = DataContext as SellerProfileViewModel;
        if (viewModel != null)
        {
            Frame.Navigate(typeof(UpdateProfileView), viewModel);
        }
    }

    private void OnSortButtonClick(object sender, RoutedEventArgs e)
    {
        var viewModel = DataContext as SellerProfileViewModel;
        if (viewModel != null)
        {
            viewModel.SortProducts();
        }
    }
}