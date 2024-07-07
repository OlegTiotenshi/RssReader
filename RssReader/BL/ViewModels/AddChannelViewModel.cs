using Rg.Plugins.Popup.Services;
using RssReader.BL.Models;
using RssReader.Helpers;
using RssReader.Services;
using System.Windows.Input;

namespace RssReader.BL.ViewModels
{
    public class AddChannelViewModel : BaseViewModel
    {
        public ChannelModel Channel
        {
            get => Get<ChannelModel>();
            set => Set(value);
        }
        public bool IsEditing;

        public ICommand SaveCommand => MakeCommand(async () =>
        {
            if (string.IsNullOrEmpty(Channel.Title) && string.IsNullOrEmpty(Channel.Uri))
            {
                await ShowAlert("Внимание", "Заполните все поля", "Ок");
                return;
            }

            bool result = IsEditing ? SQLiteService.EditChannel(Channel) : SQLiteService.AddChannel(Channel);
            if(result)
            {
                if (IsEditing)
                    MessageBus.SendMessage(Constants.ChannelEdited, Channel);

                MessageBus.SendMessage(Constants.UpdateChannels);

                CloseCommand.Execute(null);
            }
            else
            {
                await ShowAlert("Внимание", "Произошла ошибка при добавлении канала. Попробуйте еще раз.", "Ок");
            }
        });

        public ICommand CloseCommand => MakeCommand(() =>
        {
            PopupNavigation.Instance.PopAsync(true);
        });
    }
}
