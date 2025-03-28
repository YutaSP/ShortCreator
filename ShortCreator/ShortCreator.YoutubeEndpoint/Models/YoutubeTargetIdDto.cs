using Google.Apis.YouTube.v3.Data;

namespace ShortCreator.YoutubeEndpoint.Models
{
    public record YoutubeTargetIdDto(SearchResult res)
    {
        public string YoutubeVidId = res.Id.VideoId;
        public string ChannelId = res.Snippet.ChannelId;
        public string CategoryId = "";//unkown how to get at this time.
        public long Views = 100; //unknown how to get at this time.
    }
}
