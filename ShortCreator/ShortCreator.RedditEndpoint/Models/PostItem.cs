using System.ComponentModel.DataAnnotations;

namespace ShortCreator.RedditEndpoint.Models
{
    public class PostItem
    {
        [Required(ErrorMessage = "Subreddit not specified to possible subreddit")]
        public RedditPostEnums TargetSubreddit { get; set; }
        [Required(ErrorMessage = "Category for search is not specified")]
        public RedditPostSearchEnums SearchType { get; set; }
    }
}
