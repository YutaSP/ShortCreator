using ShortCreator.YoutubeEndpoint.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using ShortCreator.YoutubeEndpoint.Common;
using ShortCreator.YoutubeEndpoint.Models;

namespace ShortCreator.YoutubeEndpoint.Services
{
    public class YoutubeEndpointService : IYoutubeEndpointService
    {
        private readonly string _apiKey;
        public YoutubeEndpointService(IConfiguration configuration)
        {
            _apiKey = configuration.Required("YoutubeApiKey");
        }

        public async Task<List<YoutubeTargetIdDto>> GetTargetIds()
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = _apiKey,
                ApplicationName = this.GetType().ToString()
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = "Asmongold"; // Replace with your search term.
            searchListRequest.MaxResults = 10;

            // Call the search.list method to retrieve results matching the specified query term.
            SearchListResponse searchListResponse = await searchListRequest.ExecuteAsync();

            List<YoutubeTargetIdDto> vids = new List<YoutubeTargetIdDto>();
            foreach(SearchResult res in searchListResponse.Items)
            {
                vids.Add(new YoutubeTargetIdDto(res));
            }
            return vids;
        }

        public Task<bool> UploadVideo()
        {
            throw new NotImplementedException();
        }
    }
}
