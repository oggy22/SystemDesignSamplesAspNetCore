using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace UrlShortener.IntegrationTests
{
    [TestFixture]
    public class LiveCommentsTests
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
        public async Task SSE_Endpoint_StreamsMessages()
        {
            for (int i = 0; i < 5; i++)
                await _client.PostAsync("/comments/1", null);

            var response = await _client.GetAsync("/comments/1", HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            int count = 0;
            while (!reader.EndOfStream && count < 5)
            {
                var line = await reader.ReadLineAsync();
                if (line?.StartsWith("data:") == true)
                {
                    count++;
                }
            }

            Assert.That(count, Is.EqualTo(5));
        }
    }
}
