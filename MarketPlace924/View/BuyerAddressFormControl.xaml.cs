using MarketPlace924.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace MarketPlace924.View;

public sealed partial class BuyerAddressFormControl: UserControl
{
    public BuyerAddressFormControl()
    {
        InitializeComponent();
    }
    
    public BuyerAddressViewModel ViewModel
    {
        get => (BuyerAddressViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(BuyerAddressViewModel), typeof(BuyerAddressFormControl), new PropertyMetadata(null, OnChildViewModelChanged));

    private static void OnChildViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (BuyerAddressFormControl)d;
        control.DataContext = e.NewValue;
    }
}