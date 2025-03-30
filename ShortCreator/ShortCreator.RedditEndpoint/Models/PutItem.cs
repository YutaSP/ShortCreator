using System.ComponentModel.DataAnnotations;

namespace ShortCreator.RedditEndpoint.Models
{
    public class PutItem
    {
        [Required(ErrorMessage = "Post id not provided")]
        public string PostId { get; set; }

        [Required(ErrorMessage = "Subreddit not specified to possible subreddit")]
        public RedditPostEnums Subreddit { get; set; }

        [Required(ErrorMessage = "Category for search is not specified")]
        public RedditPostSearchEnums SearchCategory { get; set; }
    }
}
