using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RecordShop.Api.Models.DataModels;
using RecordShop.Api.Models.DTOs;
using RecordShop.Api.Repositories;
using System.Net.Http.Json;
using System.Text.Json;

namespace RecordShop.Tests.Integration
{
    public class ArtistIntegrationTests
    {
        private WebApplicationFactory<Program> _factory;

        [SetUp]
        public void SetUp()
        {
            _factory = new WebApplicationFactory<Program>();
        }

        [TearDown]
        public void TearDown()
        {
            _factory.Dispose();
        }

        [Test]
        public async Task GetAllArtistsAsyncEndpoint_ReturnsOkAndEmptyList_WhenDatabaseIsEmpty()
        {
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var mockRepo = new Mock<IArtistRepository>();

                    mockRepo.Setup(r => r.GetAllArtistsAsync()).ReturnsAsync(new List<Artist>());

                    services.AddScoped(_ => mockRepo.Object);
                });
            }).CreateClient();

            var response = await client.GetAsync("api/Artist");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var artists = JsonSerializer.Deserialize<List<GetArtistResponse>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            artists.Should().NotBeNull();
            artists.Should().BeEmpty();
        }

        [Test]
        public async Task GetAllArtistsAsyncEndpoint_ReturnsOkAndWholeList_WhenDatabaseIsSeeded()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("api/Artist");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var artists = JsonSerializer.Deserialize<List<GetArtistResponse>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            artists.Should().NotBeNull();
            artists[0].Name.Should().Be("Michael Jackson");
            artists.Should().NotBeEmpty();
        }

        [Test]
        public async Task GetArtistByIdAsyncEndpoint_ReturnsNotFound_WhenArtistDoesNotExist()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("api/Artist/10000");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Test]
        public async Task GetArtistByIdAsyncEndpoint_ReturnsOkWithArtist_WhenArtistIsFound()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("api/Artist/1");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();

            var artist = JsonSerializer.Deserialize<GetArtistResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            artist.Should().NotBeNull();
            artist.Name.Should().Be("Michael Jackson");
        }

        [Test]
        public async Task PostArtistAsyncEndpoint_ReturnsCreatedAtAction_WhenArtistDTOIsValid()
        {
            var client = _factory.CreateClient();
            var request = new PostArtistRequest("test", "test", 23);

            var response = await client.PostAsJsonAsync("api/Artist", request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

            var content = await response.Content.ReadAsStringAsync();
            var createdArtist = JsonSerializer.Deserialize<PostArtistResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            createdArtist.Should().NotBeNull();
            createdArtist.Id.Should().BeGreaterThan(0);
            createdArtist.Name.Should().Be(request.Name);
            createdArtist.Bio.Should().Be(request.Bio);
            createdArtist.Age.Should().Be(request.Age);
        }

        [Test]
        public async Task PutArtistAsyncEndpoint_ReturnsNotFound_WhenArtistNotFound()
        {
            var client = _factory.CreateClient();

            int id = 1000000;

            var requestDto = new PutArtistRequest("test", "test", 3);

            var response = await client.PutAsJsonAsync($"api/Artist/{id}", requestDto);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }
    }
}
