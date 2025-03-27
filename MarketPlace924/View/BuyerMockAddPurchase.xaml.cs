using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MarketPlace924.ViewModel;
using Microsoft.UI.Xaml.Controls;

namespace MarketPlace924.View;

public sealed partial class BuyerMockAddPurchase : UserControl
{
    public BuyerMockAddPurchase()
    {
        InitializeComponent();
    }

    public BuyerProfileViewModel? ViewModel { get; set; }


    private string? PurchaseAmount { get; set; }

    public async Task AddPurchase()
    {
        if (string.IsNullOrWhiteSpace(PurchaseAmount))
        {
            return;
        }

        var decimalPurchaseAmount = 0m;
        try
        {
            decimalPurchaseAmount = decimal.Parse(PurchaseAmount);
        }
        catch (Exception)
        {
            Debug.WriteLine("Non decimal PurchaseAmount");
        }

        if (ViewModel == null)
        {
            return;
        }

        await ViewModel.BuyerService.UpdateAfterPurchase(ViewModel.Buyer, decimalPurchaseAmount);
        ViewModel.AfterPurchase();
    }
}