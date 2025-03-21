using MarketPlace924.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace MarketPlace924.View;

public sealed partial class BuyerFamilySyncItemControl: UserControl
{
    
    public BuyerFamilySyncItemControl()
    {
        InitializeComponent();
    }
    public BuyerLinkageViewModel ViewModel
    {
        get => (BuyerLinkageViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(BuyerLinkageViewModel), typeof(BuyerFamilySyncItemControl), new PropertyMetadata(null, OnChildViewModelChanged));

    private static void OnChildViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (BuyerFamilySyncItemControl)d;
        control.DataContext = e.NewValue;
    }
}