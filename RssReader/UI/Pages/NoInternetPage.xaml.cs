using RssReader.BL.ViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace RssReader.UI.Pages
{
    public partial class NoInternetPage : PopupPage
    {
        NoInternetViewModel viewModel;
        public NoInternetPage()
        {
            InitializeComponent();

            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);

            viewModel = new NoInternetViewModel();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}