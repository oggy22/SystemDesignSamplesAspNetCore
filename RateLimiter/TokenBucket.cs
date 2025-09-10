using System;

public class TokenBucket
{
    private readonly int _capacity;          // Max tokens in bucket
    private readonly double _fillRatePerSec; // Tokens added per second
    private double _tokens;                  // Current token count
    private DateTime _lastRefill;            // Last refill time

    private readonly object _lock = new();

    public TokenBucket(int capacity, double fillRatePerSec)
    {
        _capacity = capacity;
        _fillRatePerSec = fillRatePerSec;
        _tokens = capacity; // start full
        _lastRefill = DateTime.UtcNow;
    }

    private void Refill()
    {
        var now = DateTime.UtcNow;
        var elapsed = (now - _lastRefill).TotalSeconds;
        _lastRefill = now;

        var newTokens = elapsed * _fillRatePerSec;
        _tokens = Math.Min(_capacity, _tokens + newTokens);
    }

    /// <summary>
    /// Try to consume tokens from the bucket.
    /// </summary>
    /// <param name="tokens">Number of tokens to consume</param>
    /// <returns>True if enough tokens are available</returns>
    public bool AllowRequest(int tokens = 1)
    {
        lock (_lock)
        {
            Refill();
            if (_tokens >= tokens)
            {
                _tokens -= tokens;
                return true;
            }
            return false;
        }
    }
}
