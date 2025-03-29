using Newtonsoft.Json;

namespace ShortCreator.RedditEndpoint.Models
{
    public class RootObject
    {
        [JsonProperty("data")]
        public Data Data { get; set; }
    }

    public class Data
    {
        [JsonProperty("children")]
        public List<RedditPostItem> Children { get; set; }
    }

    public class RedditPostItem
    {
        [JsonProperty("data")]
        public RedditPostDto Data { get; set; }
    }
}
