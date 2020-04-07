﻿namespace RssReader
{
    public enum NavigationMode
    {
        Normal,
        Modal,
        RootPage,
        Custom
    }

    public enum PageState
    {
        Clean,
        Loading,
        Normal,
        NoData,
        Error,
        NoInternet,
        NoAuth
    }

    public enum Pages
    {
        RssList,
        RssDetail,
        NoInternet
    }
}
