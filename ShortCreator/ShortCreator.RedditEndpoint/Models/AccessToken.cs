using Newtonsoft.Json;

namespace ShortCreator.RedditEndpoint.Models
{
    public class AccessToken
    {
        [JsonProperty("access_token")]
        public string Token { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpireTime { get; set; } // Value in seconds

        [JsonProperty("scope")]
        public string Scope { get; set; }

        public DateTime TokenExpiryDatetime { get; set; }

        public void SetTokenExpiryDatetime()
        {
            TokenExpiryDatetime = DateTime.UtcNow.AddSeconds(ExpireTime);
        }
    }
}
