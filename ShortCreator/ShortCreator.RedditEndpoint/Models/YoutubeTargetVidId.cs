using System;
using System.Collections.Generic;

namespace ShortCreator.RedditEndpoint.Models;

public partial class YoutubeTargetVidId
{
    public int? VidId { get; set; }

    public string? RedditId { get; set; }

    public string? VidPath { get; set; }
}
