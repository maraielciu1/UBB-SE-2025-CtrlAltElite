using MarketPlace924.ViewModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace MarketPlace924.View;

public sealed partial class  BuyerProfileView: Page
{ 
    public BuyerProfileViewModel? ViewModel { get; set; }

    public BuyerProfileView()
    {
        InitializeComponent();

    }
    
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is BuyerProfileViewModel viewModel)
        {
            ViewModel = viewModel;
        }
            
    }
}