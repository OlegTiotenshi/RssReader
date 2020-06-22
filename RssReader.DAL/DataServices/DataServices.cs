namespace RssReader.DAL.DataServices
{
    public static class DataServices
    {
        public static void Init()
        {
            Map = new Mock.MapDataService();
        }

        public static IMapDataService Map { get; private set; }
    }
}
