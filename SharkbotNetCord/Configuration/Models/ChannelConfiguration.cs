using MongoDB.Bson;

namespace SharkbotNetCord.Configuration.Models
{
    [Serializable]
    public class ChannelConfiguration
    {
        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public ulong ChannelId { get; set; }
        public ChannelSettings ChannelSettings { get; set; }
    }
}
