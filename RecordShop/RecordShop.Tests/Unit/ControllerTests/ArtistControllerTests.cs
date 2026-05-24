using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RecordShop.Api.Controllers;
using RecordShop.Api.Models.DTOs;
using RecordShop.Api.Services;

namespace RecordShop.Tests.Unit.ControllerTests
{
    public class ArtistControllerTests
    {
        private Mock<IArtistService> _artistServiceMock;
        private ArtistController _artistController;

        [SetUp]
        public void Setup()
        {
            _artistServiceMock = new Mock<IArtistService>();
            _artistController = new ArtistController(_artistServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _artistController.Dispose();
        }

        [Test]
        public async Task GetAllArtistsAsync_ShouldReturnOkWithEmptyList_WhenServiceReturnsEmptyList()
        {
            var testList = new List<GetArtistResponse>();

            _artistServiceMock.Setup(a => a.GetAllArtistsAsync()).ReturnsAsync(testList);

            var result = await _artistController.GetAllArtistsAsync();

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var value = okResult.Value as List<GetArtistResponse>;

            value.Should().NotBeNull();
            value.Should().BeEmpty();

            _artistServiceMock.Verify(a => a.GetAllArtistsAsync(), Times.Once());
        }

        [Test]
        public async Task GetAllArtistsAsync_ShouldReturnOkWithSeededList_WhenServiceReturnsSeededList()
        {
            var testList = new List<GetArtistResponse>
            {
                new(1, "test", "test", 2),
                new(2, "test", "test", 23),
                new(3, "test", "test", 12),
                new(4, "test", "test", 25),
                new(5, "test", "test", 50)
            };

            _artistServiceMock.Setup(a => a.GetAllArtistsAsync()).ReturnsAsync(testList);

            var result = await _artistController.GetAllArtistsAsync();

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var value = okResult.Value as List<GetArtistResponse>;

            value.Should().NotBeNull();
            value.Should().NotBeEmpty();
            value.Should().BeEquivalentTo(testList);

            _artistServiceMock.Verify(a => a.GetAllArtistsAsync(), Times.Once());  
        }

        [Test]
        public async Task GetArtistByIdAsync_ShouldReturnNotFound_WhenServiceReturnsNull()
        {
            int id = 10000;

            _artistServiceMock.Setup(a => a.GetArtistByIdAsync(id)).ReturnsAsync((GetArtistResponse)null!);

            var result = await _artistController.GetArtistByIdAsync(id);

            var notFoundResult = result.Should().BeOfType<NotFoundResult>().Subject;

            _artistServiceMock.Verify(a => a.GetArtistByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetArtistByIdAsync_ShouldReturnOkWithArtist_WhenServiceReturnsArtist()
        {
            int id = 1;

            var expectedArtist = new GetArtistResponse(id, "test", "test", 40);

            _artistServiceMock.Setup(a => a.GetArtistByIdAsync(id)).ReturnsAsync(expectedArtist);

            var result = await _artistController.GetArtistByIdAsync(id);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var value = okResult.Value as GetArtistResponse;

            value.Should().NotBeNull();
            value.Should().BeEquivalentTo(expectedArtist);
        }

        [Test]
        public async Task PostArtistAsync_ShouldReturnCreated_WhenServiceReturnsDTO()
        {
            var testRequest = new PostArtistRequest("Test", "Test", 20);
            var testResponse = new PostArtistResponse(1, "Test", "Test", 20);

            _artistServiceMock.Setup(a => a.PostArtistAsync(testRequest)).ReturnsAsync(testResponse);

            var result = await _artistController.PostArtistAsync(testRequest);

            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.RouteValues!["id"].Should().Be(1);
        }

        [Test]
        public async Task PutArtistAsync_ShouldReturnNotFound_WhenServiceReturnsNull()
        {
            int id = 1;
            var testRequest = new PutArtistRequest("Test", "Test", 20);

            _artistServiceMock.Setup(a => a.PutArtistAsync(testRequest, id)).ReturnsAsync((PutArtistResponse)null!);

            var result = await _artistController.PutArtistAsync(testRequest, id);

            var notFoundResult = result.Should().BeOfType<NotFoundResult>().Subject;

            _artistServiceMock.Verify(a => a.PutArtistAsync(testRequest, id), Times.Once());
        }

        [Test]
        public async Task PutArtistAsync_ShouldReturnCreated_WhenServiceReturnsDTOWithUpdates()
        {
            int id = 1;
            var testRequest = new PutArtistRequest("Test", "Test", 4);
            var testResponse = new PutArtistResponse(id, "Test New", "Test New", 4);

            _artistServiceMock.Setup(a => a.PutArtistAsync(testRequest, id)).ReturnsAsync(testResponse);

            var result = await _artistController.PutArtistAsync(testRequest, id);

            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.RouteValues!["id"].Should().Be(1);

            _artistServiceMock.Verify(a => a.PutArtistAsync(testRequest, id), Times.Once());
        }
    }
}
