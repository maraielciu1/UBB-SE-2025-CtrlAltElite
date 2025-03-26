using MarketPlace924.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace MarketPlace924.View;

public sealed partial class BuyerFamilySyncControl : UserControl
{
    public BuyerFamilySyncControl()
    {
        InitializeComponent();
    }

    public BuyerFamilySyncViewModel ViewModel
    {
        get => (BuyerFamilySyncViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(BuyerFamilySyncViewModel), typeof(BuyerFamilySyncControl),
            new PropertyMetadata(null, OnChildViewModelChanged));

    private static void OnChildViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (BuyerFamilySyncControl)d;
        control.DataContext = e.NewValue;
    }
}