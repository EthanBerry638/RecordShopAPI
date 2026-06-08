using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RecordShop.Api.Controllers;
using RecordShop.Api.Models.DTOs;
using RecordShop.Api.Services;

namespace RecordShop.Tests.Unit.ControllerTests
{
    public class AlbumControllerTests
    {
        private Mock<IAlbumService> _albumServiceMock;
        private AlbumController _albumController;

        [SetUp]
        public void Setup()
        {
            _albumServiceMock = new Mock<IAlbumService>();
            _albumController = new AlbumController(_albumServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _albumController.Dispose();
        }

        [Test]
        public async Task GetAllAlbumsAsync_ShouldReturnOkWithEmptyList_WhenServiceReturnsEmptyList()
        {
            var testList = new List<GetAlbumResponse>();

            _albumServiceMock.Setup(a => a.GetAllAlbumsAsync()).ReturnsAsync(testList);

            var result = await _albumController.GetAllAlbumsAsync();

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var value = okResult.Value as List<GetAlbumResponse>;

            value.Should().NotBeNull();
            value.Should().BeEmpty();

            _albumServiceMock.Verify(a => a.GetAllAlbumsAsync(), Times.Once());
        }

        [Test]
        public async Task GetAllAlbumsAsync_ShouldReturnOkWithList_WhenServiceReturnsSeededList()
        {
            var testList = new List<GetAlbumResponse>
            {
                new GetAlbumResponse
                (
                    1,
                    "Test Title1",
                    "Description for album 1",
                    new DateOnly(2020, 1, 1),
                    0.00M),
                new GetAlbumResponse
                (
                    2,
                    "Test Title2",
                    "Description for album 2",
                    new DateOnly(2021, 5, 20),
                    0.00M),
                new GetAlbumResponse
                (
                     3,
                     "Test Title3",
                     "Description for album 3",
                     new DateOnly(2022, 11, 15),
                     0.00M),
                new GetAlbumResponse
                (
                    4,
                    "Nevermind",
                    "Description for album 4",
                    new DateOnly(1991, 9, 24),
                    0.00M),
                new GetAlbumResponse
                (
                    5,
                    "Test Title5",
                    "Description for album 5",
                    new DateOnly(2023, 3, 10),
                    0.00M)
            };

            _albumServiceMock.Setup(a => a.GetAllAlbumsAsync()).ReturnsAsync(testList);

            var result = await _albumController.GetAllAlbumsAsync();

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var value = okResult.Value as List<GetAlbumResponse>;

            value.Should().NotBeNull();
            value.Should().BeEquivalentTo(testList);

            _albumServiceMock.Verify(a => a.GetAllAlbumsAsync(), Times.Once());
        }

        [Test]
        public async Task GetAlbumByIdAsync_ShouldReturnOkWithAlbum_WhenServiceReturnsAnAlbum()
        {
            var testAlbum = new GetAlbumResponse(2, "TestTitle", "Description", new DateOnly(2022, 2, 2), 12.00M);

            _albumServiceMock.Setup(a => a.GetAlbumByIdAsync(2)).ReturnsAsync(testAlbum);

            var result = await _albumController.GetAlbumByIdAsync(2);

            result.Should().BeOfType<OkObjectResult>();

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var value = okResult.Value as GetAlbumResponse;

            value.Should().NotBeNull();
            value.Should().BeEquivalentTo(testAlbum);

            _albumServiceMock.Verify(a => a.GetAlbumByIdAsync(2), Times.Once());
        }

        [Test]
        public async Task GetAlbumByIdAsync_ShouldReturnNotFound_WhenServiceReturnsNull()
        {
            _albumServiceMock.Setup(a => a.GetAlbumByIdAsync(100)).ReturnsAsync((GetAlbumResponse)null!);

            var result = await _albumController.GetAlbumByIdAsync(100);

            var notFoundResult = result.Should().BeOfType<NotFoundResult>().Subject;

            _albumServiceMock.Verify(a => a.GetAlbumByIdAsync(100), Times.Once());
        }

        [Test]
        public async Task PostAlbumAsync_ShouldReturnCreated_WhenServiceReturnsDTO()
        {
            var testRequest = new PostAlbumRequest("Test", "Test", null, 4M);
            var testResponse = new PostAlbumResponse(1, "Test", "Test", null, 4M);

            _albumServiceMock.Setup(a => a.PostAlbumAsync(testRequest)).ReturnsAsync(testResponse);

            var result = await _albumController.PostAlbumAsync(testRequest);

            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.RouteValues!["id"].Should().Be(1);

            _albumServiceMock.Verify(a => a.PostAlbumAsync(testRequest), Times.Once());
        }

        [Test]
        public async Task PutAlbumAsync_ShouldReturnOk_WhenServiceReturnsDTOWithUpdates()
        {
            int id = 1;
            var testRequest = new PutAlbumRequest("Test", "Test", null, 4M);
            var testResponse = new PutAlbumResponse(1, "Test New", "Test New", new DateOnly(2024, 2, 4), 4M);

            _albumServiceMock.Setup(a => a.PutAlbumAsync(testRequest, id)).ReturnsAsync(testResponse);

            var result = await _albumController.PutAlbumAsync(testRequest, id);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var value = okResult.Value as PutAlbumResponse;

            value.Should().NotBeNull();
            value.Should().BeEquivalentTo(testResponse);

            _albumServiceMock.Verify(a => a.PutAlbumAsync(testRequest, id), Times.Once());
        }

        [Test]
        public async Task PutAlbumAsync_ShouldReturnNotFound_WhenServiceReturnsNull()
        {
            int id = 100;
            var testRequest = new PutAlbumRequest("Test", "Test", null, 4M);

            _albumServiceMock.Setup(a => a.PutAlbumAsync(testRequest, id)).ReturnsAsync((PutAlbumResponse)null!);

            var result = await _albumController.PutAlbumAsync(testRequest, id);

            var createdResult = result.Should().BeOfType<NotFoundResult>().Subject;

            _albumServiceMock.Verify(a => a.PutAlbumAsync(testRequest, id), Times.Once());
        }

        [Test]
        public async Task DeleteAlbumByIdAsync_ShouldReturnNotFound_WhenServiceReturnsFalse()
        {
            int id = 1000000;

            _albumServiceMock.Setup(a => a.DeleteAlbumByIdAsync(id)).ReturnsAsync(false);

            var result = await _albumController.DeleteAlbumByIdAsync(id);

            var notFoundResult = result.Should().BeOfType<NotFoundResult>().Subject;

            _albumServiceMock.Verify(a => a.DeleteAlbumByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task DeleteAlbumByIdAsync_ShouldReturnNoContent_WhenServiceReturnsTrue()
        {
            int id = 1;

            _albumServiceMock.Setup(a => a.DeleteAlbumByIdAsync(id)).ReturnsAsync(true);

            var result = await _albumController.DeleteAlbumByIdAsync(id);

            var notFoundResult = result.Should().BeOfType<NoContentResult>().Subject;

            _albumServiceMock.Verify(a => a.DeleteAlbumByIdAsync(id), Times.Once());
        }
    }
}