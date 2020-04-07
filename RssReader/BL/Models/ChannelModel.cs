using SQLite;

namespace RssReader.BL.Models
{
    public class ChannelModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Uri { get; set; }
    }
}
