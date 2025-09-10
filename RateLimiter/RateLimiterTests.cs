using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace RateLimiter.IntegrationTests
{
    [TestFixture]
    public class RateLimiterTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        [SetUp]
        public void SetUp()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    var relativePath = "C:\\SystemDesignSamplesAspNetCore\\";
                    builder.UseContentRoot(relativePath);
                });
            _client = _factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Test]
        public async Task FiveOK_and_Five429()
        {
            for (int i = 0; i < 10; i++)
            {
                var response = await _client.GetAsync("/api/1");
                if (i < 5)
                {
                    Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
                }
                else
                {
                    Assert.That(response.StatusCode, Is.EqualTo((System.Net.HttpStatusCode)429));
                }
            }
        }
    }
}
