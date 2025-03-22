using Microsoft.UI.Xaml.Controls;
using MarketPlace924.Service;
using MarketPlace924.Repository;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MarketPlace924.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SellerProfileView : Page
    {
        private SellerService _sellerService;

        public SellerProfileView()
        {
            InitializeComponent();
        }
    }
}
