using ShortCreator.YoutubeEndpoint.Models;

namespace ShortCreator.YoutubeEndpoint.IServices
{
    public interface IYoutubeEndpointService
    {
        Task<List<YoutubeTargetIdDto>> GetTargetIds();
        Task<bool> UploadVideo();

    }
}
