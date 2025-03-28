using Newtonsoft.Json;

namespace ShortCreator.RedditEndpoint.Models
{
    public record RedditPostDto
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("selftext")]
        public string StoryText { get; set; }
    }
}
