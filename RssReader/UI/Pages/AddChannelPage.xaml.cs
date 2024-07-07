using RssReader.BL.ViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using RssReader.BL.Models;

namespace RssReader.UI.Pages
{
    public partial class AddChannelPage : PopupPage
    {
        AddChannelViewModel viewModel;
        
        public AddChannelPage(ChannelModel channel)
        {
            InitializeComponent();

            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);

            viewModel = new AddChannelViewModel();
            BindingContext = viewModel;

            titleLabel.Text = channel != null ? "Редактирование" : "Добавить канал";
            viewModel.IsEditing = channel != null;
            viewModel.Channel = channel ?? new ChannelModel();
        }
    }
}