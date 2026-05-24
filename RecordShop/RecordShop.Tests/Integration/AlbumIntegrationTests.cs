using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RecordShop.Api.Models.DataModels;
using RecordShop.Api.Models.DTOs;
using RecordShop.Api.Repositories;
using System.Net.Http.Json;
using System.Text.Json;

namespace RecordShop.Tests.Integration
{
    public class AlbumIntegrationTests
    {
        private WebApplicationFactory<Program> _factory;

        [SetUp]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Program>();
        }

        [TearDown]
        public void TearDown()
        {
            _factory.Dispose();
        }

        [Test]
        public async Task GetAllAlbumsAsyncEndpoint_ReturnsOkAndWholeList()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("api/Album");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var albums = JsonSerializer.Deserialize<List<GetAlbumResponse>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            albums.Should().NotBeNull();
            albums[0].Title.Should().Be("Thriller");
            albums.Should().NotBeEmpty();
        }

        [Test]
        public async Task GetAllAlbumsAsyncEndpoint_ReturnsOkAndEmptyList()
        {
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var mockRepo = new Mock<IAlbumRepository>();

                    mockRepo.Setup(r => r.GetAllAlbumsAsync()).ReturnsAsync(new List<Album>());

                    services.AddScoped(_ => mockRepo.Object);
                });
            }).CreateClient();

            var response = await client.GetAsync("api/Album");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var albums = JsonSerializer.Deserialize<List<GetAlbumResponse>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            albums.Should().NotBeNull();
            albums.Should().BeEmpty();
        }

        [Test]
        public async Task GetAllAlbumsAsyncEndpoint_ReturnsCorrectMessageFromMiddlewareWhenServerIsDown()
        {
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var mockRepo = new Mock<IAlbumRepository>();

                    mockRepo.Setup(r => r.GetAllAlbumsAsync()).ThrowsAsync(new SqliteException("Connection failed", 10));

                    services.AddScoped(_ => mockRepo.Object);
                });
            }).CreateClient();

            var response = await client.GetAsync("api/Album");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);

            var content = await response.Content.ReadAsStringAsync();

            content.Should().Contain("Server is down :(");
        }

        [Test]
        public async Task GetAlbumByIdAsyncEndpoint_ReturnsOkAndAlbumAtIndex4()
        {
            var client = _factory.CreateClient();
            var testAlbum = new Album
            {
                Id = 4,
                Title = "Nevermind",
                Description = "The definitive grunge album by Nirvana that popularized the Seattle sound globally.",
                ReleaseDate = new DateOnly(1991, 9, 24),
                Price = 5.99M
            };

            var response = await client.GetAsync("api/Album/4");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var album = JsonSerializer.Deserialize<GetAlbumResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            album.Should().NotBeNull();
            album.Should().BeEquivalentTo(testAlbum, options => options.ExcludingMissingMembers());
        }

        [Test]
        public async Task GetAlbumByIdAsyncEndpoint_ReturnsNotFound()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("api/Album/500");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Test]
        public async Task GetAlbumByIdAsyncEndpoint_ReturnsNotFound_WhenIdIsLessThanOne()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("api/Album/0");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Test]
        public async Task PostAlbumAsyncEndpoint_ReturnsCreatedAtAction()
        {
            var client = _factory.CreateClient();
            var requestDTO = new PostAlbumRequest("Test", "Test", new DateOnly(2024, 3, 4), 6);
            var expectedResponseDTO = new PostAlbumResponse(7, "Test", "Test", new DateOnly(2024, 3, 4), 6);

            var response = await client.PostAsJsonAsync("api/Album", requestDTO);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

            response.Headers.Location.Should().NotBeNull();
            response.Headers.Location.PathAndQuery.Should().Contain($"api/Album/{expectedResponseDTO.Id}");

            var content = await response.Content.ReadAsStringAsync();

            var album = JsonSerializer.Deserialize<PostAlbumResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            album.Should().NotBeNull();
            album.Should().BeEquivalentTo(expectedResponseDTO);
        }

        [Test]
        public async Task PostAlbumAsyncEndpoint_ReturnsBadRequestWithNegativePrice()
        {
            var client = _factory.CreateClient();
            var requestDTO = new PostAlbumRequest("Test", "Test", new DateOnly(2022, 3, 4), -1);

            var response = await client.PostAsJsonAsync("api/Album/", requestDTO);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task PutAlbumAsyncEndpoint_ReturnsCreatedAtAction()
        {
            var client = _factory.CreateClient();
            int id = 3;
            var requestDTO = new PutAlbumRequest("Test", "Test", new DateOnly(2012, 3, 6), 6);
            var expectedResponseDTO = new PutAlbumResponse(id, "Test", "Test", new DateOnly(2012, 3, 6), 6);

            var response = await client.PutAsJsonAsync($"api/Album/{id}", requestDTO);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

            response.Headers.Location.Should().NotBeNull();
            response.Headers.Location.PathAndQuery.Should().Contain($"api/Album/{expectedResponseDTO.Id}");

            var content = await response.Content.ReadAsStringAsync();

            var album = JsonSerializer.Deserialize<PutAlbumResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            album.Should().NotBeNull();
            album.Should().BeEquivalentTo(expectedResponseDTO);
        }

        [Test]
        public async Task PutAlbumAsyncEndpoint_ReturnsBadRequestWithNegativePrice()
        {
            var client = _factory.CreateClient();
            int id = 3;
            var requestDTO = new PutAlbumRequest("Test", "Test", new DateOnly(2024, 2, 4), -1);

            var response = await client.PutAsJsonAsync($"api/Album/{id}", requestDTO);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task PutAlbumAsyncEndpoint_ReturnsNotFoundWithIdThatDoesNotExist()
        {
            var client = _factory.CreateClient();
            int id = 10000000;
            var requestDTO = new PutAlbumRequest("Test", "Test", null, 3);

            var response = await client.PutAsJsonAsync($"api/Album/{id}", requestDTO);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Test]
        public async Task DeleteAlbumByIdAsyncEndpoint_ReturnsNotFoundWithIdThatDoesNotExist()
        {
            var client = _factory.CreateClient();
            int id = 1000000;

            var response = await client.DeleteAsync($"api/Album/{id}");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Test]
        public async Task DeleteAlbumByIdAsyncEndpoint_ReturnsNoContentWithIdThatDidExist()
        {
            var client = _factory.CreateClient();
            int id = 4;

            var response = await client.DeleteAsync($"api/Album/{id}");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }
    }
}
