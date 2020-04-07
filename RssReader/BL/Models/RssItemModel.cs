using SQLite;

namespace RssReader.BL.Models
{
    public class RssItemModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int ChannelId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public string PublishDate { get; set; }
    }
}
