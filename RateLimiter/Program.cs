var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

TokenBucket tokenBucket = new TokenBucket(5, 0.5);

app.MapGet("/api/{id}", async (HttpContext context, int id) =>
{
    string userId = "1"; // For testing purposes, hardcoded userId

    if (!tokenBucket.AllowRequest())
    {
        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        return;
    }

    context.Response.StatusCode = StatusCodes.Status200OK;
    await context.Response.WriteAsync($"Request {id} processed");
});

app.Run();
