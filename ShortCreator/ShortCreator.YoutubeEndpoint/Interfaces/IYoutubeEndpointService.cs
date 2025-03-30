using ShortCreator.YoutubeEndpoint.Models;

namespace ShortCreator.YoutubeEndpoint.Interfaces
{
    public interface IYoutubeEndpointService
    {
        Task<List<YoutubeTargetIdDto>> GetTargetIds();
        Task<bool> UploadVideo();

    }
}
