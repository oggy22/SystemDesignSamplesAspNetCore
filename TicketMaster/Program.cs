var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Get events by keyword, date range, pagination
app.MapGet("/events/search", (string? keyword, DateTime? start) =>
{
    var results = new[]
    {
        new { Id = 1, Title = "Concert", Keyword = keyword, StartDate = start }
    };

    return Results.Ok(results);
});

app.Run();
