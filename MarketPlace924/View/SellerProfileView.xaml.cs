using Microsoft.UI.Xaml.Controls;
using MarketPlace924.Service;
using MarketPlace924.Repository;
using System.Net;
using Microsoft.UI.Xaml.Navigation;
using MarketPlace924.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MarketPlace924.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SellerProfileView : Page
    {
        public SellerProfileView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is object[] parameters && parameters.Length == 2)
            {
                var userService = parameters[0] as UserService;
                var sellerService = parameters[1] as SellerService;
                DataContext = new SellerProfileViewModel(userService, sellerService, "alice_smith");
            }
        }

    }
}
