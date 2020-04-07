using Rg.Plugins.Popup.Services;
using RssReader.BL.Models;
using RssReader.Helpers;
using RssReader.Services;
using RssReader.UI.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace RssReader.BL.ViewModels
{
    public class RssListViewModel : BaseViewModel
    {
        public ObservableCollection<ChannelModel> Channels
        {
            get => Get<ObservableCollection<ChannelModel>>();
            set => Set(value);
        }
        public ChannelModel ItemSelected
        {
            get => null;
            set
            {
                if (value == null)
                    return;

                OnPropertyChanged();

                Device.BeginInvokeOnMainThread(async () =>
                {
                    await NavigateTo(Pages.RssDetail, null, NavigationMode.Normal, navParams: new Dictionary<string, object> { { "ChannelItem", value } });
                });
            }
        }

        bool isFirst = true;

        public RssListViewModel()
        {
            Channels = new ObservableCollection<ChannelModel>();
            MessagingCenter.Subscribe<MessageBus>(this, Consts.UpdateChannels, async (obj) =>
            {
                await LoadData();
            });
        }
        ~RssListViewModel()
        {
            MessagingCenter.Unsubscribe<MessageBus>(this, Consts.UpdateChannels);
        }

        protected override async Task LoadDataAsync()
        {
            if (!isFirst)
                return;

            isFirst = false;

            State = PageState.Loading;

            await LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                Channels.Clear();

                //SQLiteService.DeleteTable();
                var list = SQLiteService.GetChannels();
                foreach(var item in list)
                {
                    Channels.Add(item);
                }
            }
            catch(Exception ex)
            {

            }
            finally
            {
                State = Channels.Count != 0 ? PageState.Normal : PageState.NoData;
            }
        }

        public ICommand AddChannelCommand => MakeCommand(async () =>
        {
            await PopupNavigation.Instance.PushAsync(new AddChannelPage(null));
        });
    }
}
