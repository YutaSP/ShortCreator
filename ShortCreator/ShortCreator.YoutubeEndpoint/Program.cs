using ShortCreator.YoutubeEndpoint.Common;
using ShortCreator.YoutubeEndpoint.IServices;
using ShortCreator.YoutubeEndpoint.Models;
using ShortCreator.YoutubeEndpoint.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddTransient<IYoutubeEndpointService, YoutubeEndpointService>();

// Add services to the container.

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapGet("/youtubeEndpoint", async (IYoutubeEndpointService service) =>
{
    List<YoutubeTargetIdDto> response = await service.GetTargetIds();
    return response;
});
/*
app.MapGet("/youtubeEndpoint/{id}", (int id) =>
{
    TestDto user = new TestDto(id);

    return user.TestUser;
});
*/
app.Run();
