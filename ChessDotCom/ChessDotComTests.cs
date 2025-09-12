using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace UrlShortener.IntegrationTests
{
    [TestFixture]
    public class ChessDotComTests
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
        public async Task MatchPlayers()
        {
            //todo
        }

        [Test]
        public async Task FollowGame()
        {
            //todo
        }
    }
}
