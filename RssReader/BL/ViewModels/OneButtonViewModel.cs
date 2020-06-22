using System.Windows.Input;

namespace RssReader.BL.ViewModels
{
    public class OneButtonViewModel : BaseViewModel
    {
        public ICommand GoToMapsPageCommand => MakeCommand(async () =>
        {
            await NavigateTo(Pages.MapsPins, null, NavigationMode.Normal);
        });
    }
}
