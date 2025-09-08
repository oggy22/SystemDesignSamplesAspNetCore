using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// In-memory store
var urls = new ConcurrentDictionary<string, string>();

// Generate short code
string GenerateCode(int len = 6)
{
    const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    var random = new Random();
    return new string(Enumerable.Range(0, len).Select(_ => chars[random.Next(chars.Length)]).ToArray());
}

// Create short URL
app.MapPost("/shorten", async (HttpRequest request) =>
{
    if (request.ContentLength == 0)
        return Results.BadRequest("Request body is required.");

    var dict = await request.ReadFromJsonAsync<Dictionary<string, string>>();
    string longUrl = dict?["longUrl"];
    if (longUrl == null)
        return Results.BadRequest(new { error = "longUrl is required" });

    if (!Uri.IsWellFormedUriString(longUrl, UriKind.Absolute))
        return Results.BadRequest(new { error = "Invalid URL format" });

    var ctx = request.HttpContext;
    var code = GenerateCode();
    urls[code] = longUrl;
    var shortUrl = $"{ctx.Request.Scheme}://{ctx.Request.Host}/r/{code}";
    return Results.Ok(new { shortUrl, longUrl });
});

// Create short URL
app.MapDelete("/shorten/{url}", (HttpContext ctx) =>
{
    //if (urls.TryRemove(url, out var _))
    //    return Results.NoContent();
    var shortUrl = ctx.Request.Path;

    return Results.NotFound();
});

// Redirect
app.MapGet("/r/{code}", (string code) =>
{
    if (urls.TryGetValue(code, out var longUrl))
    {
        return Results.Redirect(longUrl);
    }
    return Results.NotFound(new { error = "Not found" });
});

// List all
app.MapGet("/list", () => urls.Select(kv => new { code = kv.Key, url = kv.Value }));

app.Run();
