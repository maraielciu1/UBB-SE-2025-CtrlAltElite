using MarketPlace924.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace MarketPlace924.View;

public sealed partial class BuyerWishlistControl: UserControl
{
    public BuyerWishlistControl()
    {
        InitializeComponent();
    }
    
    public BuyerWishlistViewModel ViewModel
    {
        get => (BuyerWishlistViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(BuyerWishlistViewModel), typeof(BuyerWishlistControl), new PropertyMetadata(null, OnChildViewModelChanged));

    private static void OnChildViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (BuyerWishlistControl)d;
        control.DataContext = e.NewValue;
    }


}