﻿using Rg.Plugins.Popup.Services;
using RssReader.BL.ViewModels;
using RssReader.Helpers;
using RssReader.UI.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RssReader.UI
{
    public sealed class NavigationService
    {
        static readonly Lazy<NavigationService> LazyInstance = new Lazy<NavigationService>(() => new NavigationService(), true);
        public static NavigationService Instance => LazyInstance.Value;

        readonly Dictionary<string, Type> _pageTypes;
        readonly Dictionary<string, Type> _viewModelTypes;

        public NavigationService()
        {
            _pageTypes = GetAssemblyPageTypes();
            _viewModelTypes = GetAssemblyViewModelTypes();
            MessagingCenter.Subscribe<MessageBus, NavigationPushInfo>(this, Consts.NavigationPushMessage, NavigationPushCallback);
            MessagingCenter.Subscribe<MessageBus, NavigationPopInfo>(this, Consts.NavigationPopMessage, NavigationPopCallback);
        }

        public static void Init(RssReader.Pages page)
        {
            Instance.Initialize(page);
        }

        void Initialize(RssReader.Pages page)
        {
            var initPage = GetInitializedPage(page.ToString());

            RootPush(initPage);
        }

        void NavigationPushCallback(MessageBus bus, NavigationPushInfo navigationPushInfo)
        {
            if (navigationPushInfo == null) throw new ArgumentNullException(nameof(navigationPushInfo));
            if (string.IsNullOrEmpty(navigationPushInfo.To)) throw new FieldAccessException(@"'To' page value should be set");

            Push(navigationPushInfo);
        }

        void NavigationPopCallback(MessageBus bus, NavigationPopInfo navigationPopInfo)
        {
            if (navigationPopInfo == null) throw new ArgumentNullException(nameof(navigationPopInfo));
            Pop(navigationPopInfo);
        }

        #region NavigationService internals
        INavigation GetTopNavigation()
        {
            var mainPage = Application.Current.MainPage;
            if (mainPage is TabbedPage tabbedPage)
            {
                if (tabbedPage.CurrentPage is NavigationPage page)
                {
                    var modalStack = page.Navigation.ModalStack.OfType<NavigationPage>().ToList();
                    if (modalStack.Any())
                    {
                        return modalStack.LastOrDefault()?.Navigation;
                    }
                    return page.Navigation;
                }
            }
            return (mainPage as NavigationPage)?.Navigation;
        }

        void Push(NavigationPushInfo pushInfo)
        {
            var newPage = GetInitializedPage(pushInfo);

            switch (pushInfo.Mode)
            {
                case NavigationMode.Normal:
                    NormalPush(newPage, pushInfo.OnCompletedTask);
                    break;
                case NavigationMode.Modal:
                    ModalPush(newPage, pushInfo.OnCompletedTask, pushInfo.NewNavigationStack);
                    break;
                case NavigationMode.RootPage:
                    RootPush(newPage, pushInfo.OnCompletedTask);
                    break;
                case NavigationMode.Custom:
                    CustomPush(newPage, pushInfo.OnCompletedTask);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        async void NormalPush(Page newPage, TaskCompletionSource<bool> completed)
        {
            //Device.BeginInvokeOnMainThread(async () =>
            //{
            try
            {
                await GetTopNavigation().PushAsync(newPage, true);
                completed.SetResult(true);
            }
            catch
            {
                completed.SetResult(false);
            }
            //});
        }

        void ModalPush(Page newPage, TaskCompletionSource<bool> completed, bool newNavigationStack = true)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    if (newNavigationStack) newPage = new NavigationPage(newPage);
                    await GetTopNavigation().PushModalAsync(newPage, true);
                    completed.SetResult(true);
                }
                catch
                {
                    completed.SetResult(false);
                }
            });
        }

        void RootPush(Page newPage, TaskCompletionSource<bool> pushInfoOnCompletedTask = null)
        {
            //Device.BeginInvokeOnMainThread(async () =>
            //{
            try
            {
                var naviPage = new NavigationPage(newPage);
                naviPage.BarTextColor = Color.White;
                Application.Current.MainPage = naviPage;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                pushInfoOnCompletedTask?.SetResult(false);
            }
            //});
        }
        void CustomPush(Page newPage, TaskCompletionSource<bool> completed, bool newNavigationStack = true)
        {

        }

        void Pop(NavigationPopInfo popInfo)
        {
            switch (popInfo.Mode)
            {
                case NavigationMode.Normal:
                    NormalPop(popInfo.OnCompletedTask);
                    break;
                case NavigationMode.Modal:
                    ModalPop(popInfo.OnCompletedTask);
                    break;
                case NavigationMode.Custom:
                    CustomPop(popInfo.OnCompletedTask);
                    break;
                case NavigationMode.RootPage:
                    RootPop(popInfo.OnCompletedTask);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        void ModalPop(TaskCompletionSource<bool> completed)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    await GetTopNavigation().PopModalAsync();
                    completed.SetResult(true);
                }
                catch
                {
                    completed.SetResult(false);
                }
            });
        }
        void CustomPop(TaskCompletionSource<bool> completed)
        {
        }
        void NormalPop(TaskCompletionSource<bool> completed)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    await GetTopNavigation().PopAsync();
                    completed.SetResult(true);
                }
                catch
                {
                    completed.SetResult(false);
                }
            });
        }

        void RootPop(TaskCompletionSource<bool> completed)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    var navigationStack = GetTopNavigation();
                    if (navigationStack.ModalStack?.Count != 0)
                    {
                        for (int i = 0; i < navigationStack.ModalStack.Count; i++)
                            await navigationStack.PopModalAsync();
                    }
                    if (navigationStack.NavigationStack?.Count != 0)
                    {
                        for (int i = 0; i < navigationStack.NavigationStack.Count; i++)
                            await navigationStack.PopAsync();
                    }
                    completed.SetResult(true);
                }
                catch
                {
                    completed.SetResult(false);
                }
            });
        }

        static string GetTypeBaseName(MemberInfo info)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            return info.Name.Replace(@"Page", "").Replace(@"ViewModel", "");
        }
        static Dictionary<string, Type> GetAssemblyPageTypes()
        {
            return typeof(BasePage).GetTypeInfo().Assembly.DefinedTypes
                .Where(ti => ti.IsClass && !ti.IsAbstract && ti.Name.Contains(@"Page") && ti.BaseType.Name.Contains(@"Page"))
                .ToDictionary(GetTypeBaseName, ti => ti.AsType());
        }
        static Dictionary<string, Type> GetAssemblyViewModelTypes()
        {
            return typeof(BaseViewModel).GetTypeInfo().Assembly.DefinedTypes
                                        .Where(ti => ti.IsClass && !ti.IsAbstract && ti.Name.Contains(@"ViewModel") &&
                                                     ti.BaseType.Name.Contains(@"ViewModel"))
                                        .ToDictionary(GetTypeBaseName, ti => ti.AsType());
        }
        BasePage GetInitializedPage(string toName, Dictionary<string, object> navParams = null)
        {
            var page = GetPage(toName);
            var viewModel = GetViewModel(toName);
            viewModel.SetNavigationParams(navParams);
            page.BindingContext = viewModel;
            return page;
        }

        Page GetInitializedPage(NavigationPushInfo navigationPushInfo)
        {
            return GetInitializedPage(navigationPushInfo.To, navigationPushInfo.NavigationParams);
        }

        BasePage GetPage(string pageName)
        {
            if (!_pageTypes.ContainsKey(pageName)) throw new KeyNotFoundException($@"Page for {pageName} not found");
            BasePage page;
            try
            {
                var pageType = _pageTypes[pageName];
                var pageObject = Activator.CreateInstance(pageType);
                page = pageObject as BasePage;
            }
            catch (Exception e)
            {
                throw new TypeLoadException($@"Unable create instance for {pageName}Page", e);
            }

            return page;
        }

        BaseViewModel GetViewModel(string pageName)
        {
            if (!_viewModelTypes.ContainsKey(pageName)) throw new KeyNotFoundException($@"ViewModel for {pageName} not found");
            BaseViewModel viewModel;
            try
            {
                viewModel = Activator.CreateInstance(_viewModelTypes[pageName]) as BaseViewModel;
            }
            catch (Exception e)
            {
                throw new TypeLoadException($@"Unable create instance for {pageName}ViewModel", e);
            }

            return viewModel;
        }
        #endregion

        public static async void GoToNoInternetPage()
        {
            try
            {
                if (PopupNavigation.Instance.PopupStack.FirstOrDefault(x => x is NoInternetPage) != null)
                    return;

                await PopupNavigation.Instance.PushAsync(new NoInternetPage());
            }
            catch(Exception ex)
            {

            }
            
        }

        public static void CloseNoInternetPage()
        {
            try
            {
                var page = PopupNavigation.Instance.PopupStack.FirstOrDefault(x => x is NoInternetPage);

                if (page != null)
                    PopupNavigation.Instance.RemovePageAsync(page, true);
            }
            catch (Exception ex)
            {

            }
        }
    }

    public class NavigationPushInfo
    {
        public string From { get; set; }
        public string To { get; set; }
        public Dictionary<string, object> NavigationParams { get; set; }
        public NavigationMode Mode { get; set; } = NavigationMode.Normal;
        public bool NewNavigationStack { get; set; }
        public TaskCompletionSource<bool> OnCompletedTask { get; set; }
    }

    public class NavigationPopInfo
    {
        public NavigationMode Mode { get; set; } = NavigationMode.Normal;
        public TaskCompletionSource<bool> OnCompletedTask { get; set; }
        public string To { get; set; }
    }
}
