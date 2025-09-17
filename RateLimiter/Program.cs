var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

TokenBucketCollection tokenBuckets = new TokenBucketCollection();
tokenBuckets.Add(new TokenBucket(5, 0.5, 1)); // UserId 1
tokenBuckets.Add(new TokenBucket(5, 0.5, 2)); // UserId 2
tokenBuckets.Add(new TokenBucket(10, 0.5, 3)); // UserId 3

// Real application will get userid from auth context or request header
app.MapGet("/api/{userid}", async (HttpContext context, int userid) =>
{
    if (!tokenBuckets[userid].AllowRequest())
    {
        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        return;
    }

    context.Response.StatusCode = StatusCodes.Status200OK;
    await context.Response.WriteAsync($"Request for userid={userid} processed");
});

app.Run();
