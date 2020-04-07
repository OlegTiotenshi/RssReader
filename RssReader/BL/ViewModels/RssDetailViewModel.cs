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
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RssReader.BL.ViewModels
{
    public class RssDetailViewModel : BaseViewModel
    {
        public string ChannelTitle
        {
            get => Get<string>();
            set => Set(value);
        }
        public bool IsRefreshing
        {
            get => Get(false);
            set => Set(value);
        }
        public ObservableCollection<RssItemModel> RssItems
        {
            get => Get<ObservableCollection<RssItemModel>>();
            set => Set(value);
        }

        public RssItemModel ItemSelected
        {
            get => null;
            set
            {
                if (value == null)
                    return;

                OnPropertyChanged();

                Launcher.OpenAsync(new Uri(value.Link));
            }
        }

        ChannelModel channelModel;
        bool isFirst = true;

        public RssDetailViewModel()
        {
            RssItems = new ObservableCollection<RssItemModel>();

            MessagingCenter.Subscribe<MessageBus, ChannelModel>(this, Consts.ChannelEdited, async (obj, data) =>
            {
                ChannelTitle = data.Title;
                channelModel = data;

                if (IsConnected)
                    await LoadData();
                else
                    await UploadData();
            });
        }
        ~RssDetailViewModel()
        {
            MessagingCenter.Unsubscribe<MessageBus, ChannelModel>(this, Consts.ChannelEdited);
        }

        public override void OnSetNavigationParams(Dictionary<string, object> navigationParams)
        {
            base.OnSetNavigationParams(navigationParams);
            navigationParams.TryGetValue("ChannelItem", out channelModel);

            ChannelTitle = channelModel.Title;
        }

        protected override async Task LoadDataAsync()
        {
            if (!isFirst)
                return;

            isFirst = false;

            State = PageState.Loading;

            if (IsConnected)
                await LoadData();
            else
                await UploadData();
        }

        private async Task LoadData()
        {
            if (!IsConnected)
            {
                IsRefreshing = false;
                State = PageState.NoInternet;
                return;
            }
            IsRefreshing = true;

            RssItems.Clear();

            try
            {
                var response = await RestService.GetRssDetails(channelModel.Uri);
                if (response != null)
                {
                    var list = await XmlParseService.Parser(response);
                    foreach (var item in list)
                    {
                        item.ChannelId = channelModel.Id;
                        RssItems.Add(item);
                    }
                }

            }
            catch (Exception ex)
            {
            }
            finally
            {
                State = RssItems.Count != 0 ? PageState.Normal : PageState.NoData;
                IsRefreshing = false;
            }
        }

        private async Task UploadData()
        {
            IsRefreshing = true;
            try
            {
                RssItems.Clear();

                var list = SQLiteService.GetRssItems(channelModel.Id);
                foreach (var item in list)
                {
                    item.ChannelId = channelModel.Id;
                    RssItems.Add(item);
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                State = RssItems.Count != 0 ? PageState.Normal : PageState.NoData;
                IsRefreshing = false;
            }
        }

        public ICommand RefreshCommand => MakeCommand(async () =>
        {
            if (IsRefreshing)
                return;

            if (IsConnected)
                await LoadData();
            else
                await UploadData();
        });

        public ICommand BackCommand => MakeCommand(async () =>
        {
            GoBackCommand.Execute(null);

            SQLiteService.DeleteRssItems(channelModel.Id);
            SQLiteService.AddRssItems(RssItems);
        });

        public ICommand OperationCommand => MakeCommand(async () =>
        {
            var result = await ShowSheet("Выберите действие", "Отмена", null, new string[] { "Редактирование", "Удалить" });
            if (result == null || result.Equals("Выберите действие"))
                return;

            switch (result)
            {
                case "Редактирование":
                    EditOperation();
                    break;
                case "Удалить":
                    DeleteOperation();
                    break;
                default:
                    break;
            }
        });

        async void DeleteOperation()
        {
            var result = await ShowQuestion("Внимание", "Вы уверены, что хотите удалить данный канал из своего списка?", "Принять", "Отмена");

            if (result)
            {
                SQLiteService.DeleteChannel(channelModel.Id);
                MessageBus.SendMessage("UpdateChannels");

                GoBackCommand.Execute(null);
            }
        }

        async void EditOperation()
        {
            await PopupNavigation.Instance.PushAsync(new AddChannelPage(channelModel));
        }
    }
}
