using System.Collections.Concurrent;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

ConcurrentDictionary<int, Channel<string>> liveComments = new();

app.MapPost("/comments/{liveVideoId}", (int liveVideoId) =>
{
    liveComments.GetOrAdd(liveVideoId, _ => Channel.CreateUnbounded<string>(new UnboundedChannelOptions
    {
        SingleWriter = true,
        SingleReader = false
    }));
    string comment = $"Comment at {DateTime.UtcNow}";
    liveComments[liveVideoId].Writer.TryWrite(comment);
    return Results.Ok($"Comment posted for video {liveVideoId}");
});

// SSE endpoint to stream comments
app.MapGet("/comments/{liveVideoId}", async (HttpContext context, int liveVideoId) =>
{
    context.Response.Headers.Add("Content-Type", "text/event-stream");
    context.Response.Headers.Add("Cache-Control", "no-cache");
    context.Response.Headers.Add("Connection", "keep-alive");

    var cancellationToken = context.RequestAborted; // Cancel if client disconnects

    await foreach (var comment in liveComments[liveVideoId].Reader.ReadAllAsync(cancellationToken))
    {
        var json = System.Text.Json.JsonSerializer.Serialize(comment);
        await context.Response.WriteAsync($"data: {json}\n\n", cancellationToken);
        await context.Response.Body.FlushAsync(cancellationToken);
    }
});

app.Run();
