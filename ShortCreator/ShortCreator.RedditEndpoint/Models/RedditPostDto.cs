using System.Text.Json.Serialization;
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

        //below for exclusion of the video only

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("post_hint")]
        public string PostHint { get; set; }

        [JsonPropertyName("is_video")]
        public bool IsVideo { get; set; }
    }
}
