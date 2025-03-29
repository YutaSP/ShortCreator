using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.Features;
using Newtonsoft.Json;
using ShortCreator.RedditEndpoint.Data;
using ShortCreator.RedditEndpoint.Models;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
//added myself, not sure if this is necessary but this shoudl be added to any http server,
//idk how this will translate for gRPC.cross that bridge when i get to it.
//adding proper response detail for endpoint, possibly move this up to service defaults
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
        var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
    };
});
// Add services to the container.
//below is describe as singleton but should be scoped or transient.
//if moving to scoped or transient, must add auth token service to ensure token is somehting retained through out app but calls itself shoudl be transient
builder.Services.AddSingleton<IRedditApiService, RedditApiService>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();


app.MapGet("/redditEndpoint", async (ILogger<Program> logger) =>
{
    //below is test rewrite to get data from db for story to be consumed by other apps.
    var agent = "ShortMakerScript/1.0 (Windows 11) by /u/Pretty_Will_1754";
    var subreddit = "COD";
    try
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", agent);
            string url = $"https://www.reddit.com/r/{subreddit}/about.json";
            var response = await client.GetStringAsync(url);

            // Deserialize the JSON response into an object
            var subredditInfo = JsonConvert.DeserializeObject<object>(response);
            logger.LogInformation("Information received {response}", response);
            // Return the data from the response
            return Results.Ok(subredditInfo);
        }
    }
    catch (Exception ex)
    {
        //log out exception
        return Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    type: "Bad Request",
                    detail: ex.Message
                );
    }
});

app.MapPost(
    "redditEndpoint", 
    async (PostItem postItem, ILogger<Program> logger, IRedditApiService redditApiService) =>
{
    var validationConext = new ValidationContext(postItem);
    var validationResult = new List<ValidationResult>();
    var isValid = Validator.TryValidateObject(postItem, validationConext, validationResult, true);

    if (!isValid)
    {
        string resultString = string.Empty;
        foreach(var validation in validationResult)
        {
            resultString += string.Format("{0}; ", validation.ErrorMessage);
        }

        return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                type: "Bad Request",
                detail: resultString
            );
    }

    //make sure the category is not capitalized
    var post = await redditApiService.GetPost(postItem.TargetSubreddit.ToString(), postItem.SearchType.ToString());

    return Results.Ok();
});

app.Run();


