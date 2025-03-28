var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.ShortCreator_ApiService>("apiservice");

builder.AddNpmApp("react", "../ShortCreator.Frontend")
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithEnvironment("BROWSER", "none") // Disable opening browser on npm start
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.AddProject<Projects.ShortCreator_YoutubeEndpoint>("shortcreator-youtubeendpoint");

builder.AddProject<Projects.ShortCreator_RedditEndpoint>("shortcreator-redditendpoint");

builder.Build().Run();
