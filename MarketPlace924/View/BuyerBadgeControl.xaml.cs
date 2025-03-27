using MarketPlace924.Domain;
using MarketPlace924.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace MarketPlace924.View;

public sealed partial class BuyerBadgeControl : UserControl
{
    public BuyerBadgeControl()
    {
        InitializeComponent();
    }



    public BuyerBadgeViewModel ViewModel
    {
        get => (BuyerBadgeViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }


    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(BuyerBadgeViewModel), typeof(Buyer), typeof(BuyerBadgeControl),
            new PropertyMetadata(null, OnChildViewModelChanged));

    private static void OnChildViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (BuyerBadgeControl)d;
        control.DataContext = e.NewValue;
    }
}