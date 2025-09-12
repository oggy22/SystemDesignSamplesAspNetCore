using System.Collections.Concurrent;
using System.Threading.Channels;
using ChessDotCom;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

ConcurrentDictionary<int, Channel<string>> liveComments = new();

var people = new UserCollection
{
    new User { Id = 1, Name = "Magnus", Rating=2600 },
    new User { Id = 2, Name = "Hikaru", Rating=2500 },
    new User { Id = 3, Name = "Kasparov", Rating=2550 },
    new User { Id = 4, Name = "Zare", Rating=2000 }
};

// Create a new game, i.e. request a match with another player
app.MapPost("/games/", (int liveVideoId) =>
{
    return Results.Ok($"Game created");
});

// Make a move for a game
app.MapPost("/games/{gameId}/moves", (int liveVideoId) =>
{
    // todo
    return Results.Ok($"Game created");
});


app.MapGet("/games/{gameId}/moves", async (HttpContext context, int gameId) =>
{
    // todo
});

app.Run();
