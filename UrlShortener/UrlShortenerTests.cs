using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Net;
using System.Text;

namespace UrlShortener.IntegrationTests
{
    [TestFixture]
    public class UrlShortenerTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        [SetUp]
        public void SetUp()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseContentRoot("\\CsRestServer");
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
        public async Task ShortenUrl_ReturnsShortenedUrl()
        {
            const string longURL = "https://example.com";

            // Create short URL from long URL
            var requestBody = new JObject { ["longUrl"] = longURL };
            var content = new StringContent(requestBody.ToString(), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/shorten", content);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            // Get the short URL from the response
            var responseBody = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(responseBody);
            string shortUrl = json["shortUrl"].Value<string>();
            Assert.That(shortUrl, Is.Not.Null.And.Not.Empty);

            // Verify that the short URL redirects to the original long URL
            var response2 = await _client.GetAsync(shortUrl);
            Assert.That(response2.RequestMessage.RequestUri, Is.EqualTo(longURL));
        }

        [Test]
        public async Task ShortenUrl_InvalidUrl_ReturnsBadRequest()
        {
            var requestBody = new JObject { ["longUrl"] = "not-a-valid-url" };
            var content = new StringContent(requestBody.ToString(), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/shorten", content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task ShortenUrl_EmptyBody_ReturnsBadRequest()
        {
            var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/shorten", content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }
}
