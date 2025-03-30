using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShortCreator.RedditEndpoint.Data;
using ShortCreator.RedditEndpoint.Models;
using ShortCreator.RedditEndpoint.Service;
using ShortCreator.YoutubeEndpoint.Common;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
//below is describe as singleton but should be scoped or transient.
//if moving to scoped or transient, must add auth token service to ensure token is somehting retained through out app but calls itself shoudl be transient
builder.Services.AddSingleton<IRedditApiService, RedditApiService>();

builder.Services.AddDbContext<RedditDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.Required("DbConnectionString"));
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();


app.MapGet("/redditEndpoint", async (ILogger<Program> logger, RedditDbContext dbContext) =>
{
    try
    {
        var story = await dbContext.RedditStories.Take(1).SingleOrDefaultAsync();

        if (story == null)
            throw new Exception("No reddit story to provide");

        return Results.Ok(story);
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
    "/redditEndpoint", 
    async (PostItem postItem, ILogger<Program> logger, IRedditApiService redditApiService, RedditDbContext dbContext) =>
{
    try
    {
        //make sure the category is not capitalized
        var validationConext = new ValidationContext(postItem);
        var validationResult = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(postItem, validationConext, validationResult, true);

        if (!isValid)
        {
            string resultString = string.Empty;
            foreach (var validation in validationResult)
            {
                resultString += string.Format("{0}; ", validation.ErrorMessage);
            }

            throw new ValidationException(resultString);
        }

        var post = await redditApiService.GetPost(postItem.TargetSubreddit.ToString(), postItem.SearchType.ToString());

        //should make mapper if data handling is bigger than this
        var story = new RedditStory
        {
            Id = post.ID,
            Title = post.Title,
            Story = post.StoryText
        };
        //insert said post
        dbContext.RedditStories.Add(story);
        await dbContext.SaveChangesAsync();

        return Results.Ok(story.Id);
    }
    catch(ValidationException ex)
    {
        logger.LogInformation(ex.Message);

        return Results.Problem(
            statusCode: StatusCodes.Status400BadRequest,
            type: "Bad Request",
            detail: ex.Message
        );
    }
    catch (Exception ex)
    {
        logger.LogInformation(ex.Message);

        return Results.Problem(
            statusCode: StatusCodes.Status400BadRequest,
            type: "Bad Request",
            detail: ex.Message
        );
    }
});

app.MapDelete("/redditEndpoint/{id}", async (int id, ILogger<Program> logger, RedditDbContext dbContext) =>
{
    try
    {
        var post = await dbContext.RedditStories.SingleAsync(e => e.Id.Equals(id));
        dbContext.Remove(post);
        await dbContext.SaveChangesAsync();

        return Results.Ok(id);
    }
    catch (Exception ex)
    {
        logger.LogInformation(ex.Message);

        return Results.Problem(
            statusCode: StatusCodes.Status400BadRequest,
            type: "Bad Request",
            detail: ex.Message
        );
    }
});
app.MapPut("/redditEndpoint", async (PutItem item, ILogger<Program> logger, RedditDbContext dbContext) =>
{
    try
    {
        //make sure the category is not capitalized
        var validationConext = new ValidationContext(item);
        var validationResult = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(item, validationConext, validationResult, true);

        if (!isValid)
        {
            string resultString = string.Empty;
            foreach (var validation in validationResult)
            {
                resultString += string.Format("{0}; ", validation.ErrorMessage);
            }

            throw new ValidationException(resultString);
        }

        var usedStory = new RedditUsedStory
        {
            Id = item.PostId,
            SubredditEnum = (int?)item.Subreddit,
            CategoryEnum = (int?)item.SearchCategory,
        };

        var updateTarget = await dbContext.RedditUsedStories.FirstOrDefaultAsync(x => x.SubredditEnum == usedStory.SubredditEnum);

        if (updateTarget != null)
        {
            updateTarget.Id = usedStory.Id;
            updateTarget.CategoryEnum = usedStory.CategoryEnum;
            dbContext.Update(updateTarget);

            return Results.Ok(await dbContext.SaveChangesAsync());
        }

        await dbContext.AddAsync(usedStory);

        return Results.Ok(await dbContext.SaveChangesAsync());
    }
    catch (ValidationException ex)
    {
        logger.LogInformation(ex.Message);

        return Results.Problem(
            statusCode: StatusCodes.Status400BadRequest,
            type: "Bad Request",
            detail: ex.Message
        );
    }
    catch (Exception ex)
    {
        logger.LogInformation(ex.Message);

        return Results.Problem(
            statusCode: StatusCodes.Status400BadRequest,
            type: "Bad Request",
            detail: ex.Message
        );
    }
});

app.Run();


