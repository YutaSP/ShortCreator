
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using ShortCreator.RedditEndpoint.Models;
using ShortCreator.YoutubeEndpoint.Common;

namespace ShortCreator.RedditEndpoint.Service
{
    public class RedditApiService : IRedditApiService
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _user;
        private readonly string _pass;

        private AccessToken _accessToken;

        private ILogger<RedditApiService> _logger;

        public RedditApiService(ILogger<RedditApiService> logger, IConfiguration config)
        {
            _clientId = config.Required("RedditClientId");
            _clientSecret = config.Required("RedditKey");
            _user = config.Required("User");
            _pass = config.Required("Pass");
            _logger = logger;
        }

        public async Task<RedditPostDto?> GetPost(string subreddit, string category, string after = "")
        {
            if(_accessToken == null || DateTime.UtcNow > _accessToken.TokenExpiryDatetime || string.IsNullOrEmpty(_accessToken.Token))
            {
                await GetAccessTokenAsync();
            }

            if(_accessToken != null && !string.IsNullOrEmpty(_accessToken.Token))
            {
                try
                {
                    using(HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken.Token);

                        var agent = "ShortMaker/1.0";
                        client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", agent);

                        string url = $"https://oauth.reddit.com/r/{subreddit}/{category}?limit=1";
                        if (!string.IsNullOrEmpty(after))
                        {
                            url += $"&after={after}";
                        }

                        var response = await client.GetAsync(url);

                        if (!response.IsSuccessStatusCode)
                        {
                            throw new HttpRequestException();
                        }

                        var content = await response.Content.ReadAsStringAsync();

                        var root = JsonConvert.DeserializeObject<RootObject>(content);
                        if (root == null)
                        {
                            throw new Exception("Unable to deserialize content from the reddit post");
                        }

                        // Get the first Reddit post
                        var firstPost = root.Data.Children[1].Data;

                        // Check if post is an image or video and skip it
                        if (IsMediaPost(firstPost.PostHint, firstPost.IsVideo, firstPost.Url))
                        {
                            //the post should be skipped
                            _logger.LogInformation("The post retrieved was skipped. Reason: Media type not story");
                            return null;
                        }


                        string cleanedText = firstPost.StoryText.Replace("\\n", " ").Replace("\\\"", "\"");

                        // Optionally, remove any extra spaces that could have been introduced
                        cleanedText = System.Text.RegularExpressions.Regex.Replace(cleanedText, @"\s+", " ").Trim();

                        // Create a RedditPostDto from the first post
                        RedditPostDto post = new RedditPostDto
                        {
                            ID = firstPost.ID,
                            Title = firstPost.Title,
                            StoryText = cleanedText
                        };

                        if (post == null || string.IsNullOrEmpty(post.ID) || string.IsNullOrEmpty(post.Title) || string.IsNullOrEmpty(post.StoryText))
                        {
                            throw new Exception("The dto contains null or is null, fundamental issue with seeing the post");
                        }

                        return post;
                    }
                }
                catch(HttpRequestException ex)
                {
                    _logger.LogCritical("HTTP exception occurred, ensure network outage not occuring and able to hit reddit from browser. Error: {}", ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex.Message);
                }
            }

            return null;
        }

        // Reddit's OAuth2 password grant flow
        private async Task GetAccessTokenAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // Create the basic authorization header
                    var agent = "ShortMaker/1.0";
                    client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", agent);
                    var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);

                    // Add the User-Agent header (Reddit requires it)
                    client.DefaultRequestHeaders.Add("User-Agent", "YourAppName/1.0");

                    // Prepare the data for the POST request
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("grant_type", "password"),
                        new KeyValuePair<string, string>("username", _user),
                        new KeyValuePair<string, string>("password", _pass)
                    });

                    // Send the request to get the access token
                    var response = await client.PostAsync("https://www.reddit.com/api/v1/access_token", formContent);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Request to reddit for auth failed with status of {response.StatusCode.ToString()}");
                    }
                    var content = await response.Content.ReadAsStringAsync();

                    _accessToken = JsonConvert.DeserializeObject<AccessToken>(content);

                    if (_accessToken == null || string.IsNullOrEmpty(_accessToken.Token))
                    {
                        throw new Exception($"Access token is null or empty, there might be something wrong with reddit");
                    }
                    _accessToken.SetTokenExpiryDatetime();
                    //content;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }            
        }
        private bool IsMediaPost(string postHint, bool isVideo, string url)
        {
            // Exclude posts that are images or videos
            return postHint == "image" || postHint == "hosted:video" || isVideo || IsMediaUrl(url);
        }

        private bool IsMediaUrl(string url)
        {
            // Check if URL links directly to an image or video file
            return url.EndsWith(".jpg") || url.EndsWith(".png") || url.EndsWith(".gif") ||
                   url.EndsWith(".mp4") || url.EndsWith(".webm") || url.Contains("gfycat.com") ||
                   url.Contains("imgur.com") && !url.Contains("/a/"); // Exclude Imgur album links
        }
    }
}
