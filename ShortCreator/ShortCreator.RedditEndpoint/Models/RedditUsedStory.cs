using System;
using System.Collections.Generic;

namespace ShortCreator.RedditEndpoint.Models;

public partial class RedditUsedStory
{
    public string? Id { get; set; }

    public int? SubredditEnum { get; set; }

    public int? CategoryEnum { get; set; }
}
