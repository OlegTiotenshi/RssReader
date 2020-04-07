using RssReader.Services;
using RssReader.UI;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RssReader
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            DialogService.Init(this);

            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            
            NavigationService.Init(Pages.RssList);
        }
        public App(string databaseLocation) : this()
        {
            SQLiteService.Init(databaseLocation);
        }

        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess != NetworkAccess.Internet)
                NavigationService.GoToNoInternetPage();
            else
                NavigationService.CloseNoInternetPage();
        }

        protected override void OnStart()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                NavigationService.GoToNoInternetPage();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
