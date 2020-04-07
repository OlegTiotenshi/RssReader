using RssReader.BL.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RssReader.Services
{
    public class SQLiteService
    {
        static readonly Lazy<SQLiteService> LazyInstance = new Lazy<SQLiteService>(() => new SQLiteService(), true);
        public static SQLiteService Instance => LazyInstance.Value;
        static SQLiteConnection conn;

        public static void Init(string databaseLocation)
        {
            Instance.SetApp(databaseLocation);
        }
        void SetApp(string databaseLocation)
        {
            conn = new SQLiteConnection(databaseLocation);
            conn.CreateTable<ChannelModel>();
            conn.CreateTable<RssItemModel>();
        }

        public static IEnumerable<ChannelModel> GetChannels()
        {
            return conn.Table<ChannelModel>().ToList().OrderByDescending(p => p.Id);
        }

        public static bool AddChannel(ChannelModel channel)
        {
            int result = conn.Insert(channel);
            return result > 0;
        }
        public static bool EditChannel(ChannelModel channel)
        {
            int result = conn.Update(channel);
            return result > 0;
        }
        public static bool DeleteChannel(int channelId)
        {
            var channelsList = conn.Table<ChannelModel>().ToList();
            var search = channelsList.FirstOrDefault(x => x.Id == channelId);

            if (search != null)
            {
                int rows = conn.Delete(search);
                if (rows > 0)
                {
                    DeleteRssItems(channelId);

                    return true;
                }
                else return false;
            }

            return false;
        }

        public static IEnumerable<RssItemModel> GetRssItems(int channelId)
        {
            return conn.Table<RssItemModel>().Where(x => x.ChannelId == channelId).ToList();
        }
        public static void AddRssItems(ObservableCollection<RssItemModel> items)
        {
            foreach (var item in items)
                conn.Insert(item);
        }
        public static void DeleteRssItems(int channelId)
        {
            var itemsList = conn.Table<RssItemModel>().ToList();
            foreach (var item in itemsList)
            {
                if (item.ChannelId == channelId)
                    conn.Delete(item);
            }
        }



        public static void DeleteTable()
        {
            conn.DeleteAll<ChannelModel>();
        }
    }
}
