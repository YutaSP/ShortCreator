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
        public List<RedditPost> Children { get; set; }
    }

    public class RedditPost
    {
        [JsonProperty("data")]
        public RedditPostDto Data { get; set; }
    }
}
