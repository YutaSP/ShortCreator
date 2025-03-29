using ShortCreator.RedditEndpoint.Models;

namespace ShortCreator.RedditEndpoint.Service
{
    public interface IRedditApiService
    {
        Task<RedditPostDto?> GetPost(string subreddit, string category, string after = "");

    }
}
