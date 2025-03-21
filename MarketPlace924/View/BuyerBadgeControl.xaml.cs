using System;
using MarketPlace924.Domain;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace MarketPlace924.View;

public sealed partial class BuyerBadgeControl : UserControl
{
    public BuyerBadgeControl()
    {
        InitializeComponent();
    }


    private int Progress => (int)(Math.Min(1.0m,
        ((Buyer.TotalSpending / 1000.0m) * 0.8m + (Buyer.NumberOfPurchases / 100.0m) * 0.2m)) * 100);

    private string BadgeName => Buyer.Badge.ToString().ToLower();

    private string ImageSource => "ms-appx:///Assets/badge-" + Buyer.Badge.ToString().ToLower() + ".png";

    public Buyer Buyer
    {
        get => (Buyer)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }


    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(Buyer), typeof(Buyer), typeof(BuyerBadgeControl),
            new PropertyMetadata(null, OnChildViewModelChanged));

    private static void OnChildViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (BuyerBadgeControl)d;
        control.DataContext = e.NewValue;
    }
}