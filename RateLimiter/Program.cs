using System.Collections.Generic;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

TokenBucketCollection tokenBuckets =
[
    new TokenBucket(5, 0.5, 1), // UserId 1
    new TokenBucket(5, 0.5, 2), // UserId 2
    new TokenBucket(10, 0.5, 3), // UserId 3
];

// Real application will get userid from auth context or request header
app.MapGet("/api/{userid}", async (HttpContext context, int userid) =>
{
    var tokenBucket = tokenBuckets.Contains(userid) ? tokenBuckets[userid] : null;
    if (tokenBucket!=null && !tokenBucket.AllowRequest())
    {
        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.Response.Headers["X-RateLimit-Limit"] = tokenBucket.Limit.ToString();
        context.Response.Headers["X-RateLimit-Remaining"] = "0";
        context.Response.Headers["X-RateLimit-Reset"] = DateTime.UtcNow.AddSeconds(tokenBucket.Reset()).ToString("R");
        return;
    }

    context.Response.StatusCode = StatusCodes.Status200OK;
    if (tokenBucket != null)
        context.Response.Headers["X-RateLimit-Remaining"] = tokenBucket.Remaining().ToString();
    await context.Response.WriteAsync($"Request for userid={userid} processed");
});

app.Run();
