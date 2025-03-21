using MarketPlace924.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace MarketPlace924.View;

public sealed partial  class BuyerWishlistItemControl : UserControl
{
    
    public BuyerWishlistItemControl()
    {
        InitializeComponent();
    }
    public BuyerWishlistItemViewModel ViewModel
    {
        get => (BuyerWishlistItemViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(BuyerWishlistItemViewModel), typeof(BuyerWishlistItemControl), new PropertyMetadata(null, OnChildViewModelChanged));

    private static void OnChildViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (BuyerWishlistItemControl)d;
        control.DataContext = e.NewValue;
    }
}