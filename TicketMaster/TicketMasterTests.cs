using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace TicketMaster.IntegrationTests
{
    [TestFixture]
    public class TicketMasterTests
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
        public async Task GetEvents()
        {
            var response = await _client.GetAsync("/events/search?keyword=tyler", HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
        }
    }
}
