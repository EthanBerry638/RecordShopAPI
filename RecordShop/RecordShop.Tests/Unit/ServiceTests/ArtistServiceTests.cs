using FluentAssertions;
using Moq;
using RecordShop.Api.Models.DataModels;
using RecordShop.Api.Models.DTOs;
using RecordShop.Api.Repositories;
using RecordShop.Api.Services;
using System.Threading.Tasks;

namespace RecordShop.Tests.Unit.ServiceTests
{
    public class ArtistServiceTests
    {
        private Mock<IArtistRepository> _artistRepositoryMock;
        private ArtistService _artistService;

        [SetUp]
        public void Setup()
        {
            _artistRepositoryMock = new Mock<IArtistRepository>();
            _artistService = new ArtistService(_artistRepositoryMock.Object);
        }

        [Test]
        public async Task GetAllArtistsAsync_ShouldReturnEmptyDTOList_WhenRepoMethodIsCalledAndDBIsNotSeeded()
        {
            var expectedList = new List<Artist>();

            _artistRepositoryMock.Setup(a => a.GetAllArtistsAsync()).ReturnsAsync(expectedList);

            var result = await _artistService.GetAllArtistsAsync();

            result.Should().BeEmpty();
            result.Should().BeEquivalentTo(expectedList);

            _artistRepositoryMock.Verify(a => a.GetAllArtistsAsync(), Times.Once());
        }

        [Test]
        public async Task GetAllArtistsAsync_ShouldReturnSeededDTOList_WhenRepoMethodIsCalledAndDBIsSeeded()
        {
            var expectedList = new List<Artist>
            {
                new() { Id = 1, Name = "Test Artist1", Bio = "Blah blah", Age = 20},
                new() { Id = 2, Name = "Test Artist2", Bio = "Blah blah", Age = 20},
                new() { Id = 3, Name = "Test Artist3", Bio = "Blah blah", Age = 20},
                new() { Id = 4, Name = "Test Artist4", Bio = "Blah blah", Age = 20},
                new() { Id = 5, Name = "Test Artist5", Bio = "Blah blah", Age = 20}
            };

            var expectedDtos = new List<GetArtistResponse>
            {
                new(1,"Test Artist1","Blah blah",20),
                new(2,"Test Artist2","Blah blah",20),
                new(3,"Test Artist3","Blah blah",20),
                new(4,"Test Artist4","Blah blah",20),
                new(5,"Test Artist5","Blah blah",20)
            };

            _artistRepositoryMock.Setup(a => a.GetAllArtistsAsync()).ReturnsAsync(expectedList);

            var result = await _artistService.GetAllArtistsAsync();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expectedDtos);
            result.Count.Should().Be(5);

            _artistRepositoryMock.Verify(a => a.GetAllArtistsAsync(), Times.Once());
        }

        [Test]
        public async Task GetArtistByIdAsync_ShouldReturnNullAndNotThrowAnException_WhenRepoMethodIsCalledAndReturnsNull()
        {
            int id = 10000;

            _artistRepositoryMock.Setup(a => a.GetArtistByIdAsync(id)).ReturnsAsync((Artist)null!);

            var result = await _artistService.GetArtistByIdAsync(id);

            result.Should().BeNull();

            _artistRepositoryMock.Verify(a => a.GetArtistByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetArtistByIdAsync_ShouldMapArtistToDTO_WhenRepoMethodIsCalledAndReturnsAnArtist()
        {
            int id = 1;

            var artist = new Artist { Id = id, Name = "Test Name", Bio = "Test Bio", Age = 20 };
            var artistDto = new GetArtistResponse(id, "Test Name", "Test Bio", 20);

            _artistRepositoryMock.Setup(a => a.GetArtistByIdAsync(id)).ReturnsAsync(artist);

            var result = await _artistService.GetArtistByIdAsync(id);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(artistDto);

            _artistRepositoryMock.Verify(a => a.GetArtistByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task PostArtistAsync_ShouldMapDtoAndReturnResponse_WhenValidRequestGiven()
        {
            var requestDto = new PostArtistRequest("Test", "Test", 60);

            var expectedArtist = new Artist
            {
                Id = 1,
                Name = requestDto.Name,
                Bio = requestDto.Bio,
                Age = requestDto.Age,
            };

            _artistRepositoryMock.Setup(a => a.PostArtistAsync(It.IsAny<Artist>())).ReturnsAsync(expectedArtist);

            var result = await _artistService.PostArtistAsync(requestDto);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Should().BeEquivalentTo(expectedArtist, options => options.ExcludingMissingMembers());

            _artistRepositoryMock.Verify(a => a.PostArtistAsync(It.IsAny<Artist>()), Times.Once());
        }

        [Test]
        public async Task PutArtistAsync_ShouldNotThrowExcepetionAndReturnNull_WhenGetRequestCannotFindArtistUpdate()
        {
            var requestDto = new PutArtistRequest("Test", "Test", 60);
            int id = 1;

            var result = await _artistService.PutArtistAsync(requestDto, id);

            result.Should().BeNull();
        }

        [Test]
        public async Task PutArtistAsync_ShouldUpdateAndReturnResponse_WhenArtistExistsAndGetRequestCanFindArtist()
        {
            var requestDto = new PutArtistRequest("Updated Name", "Updated Bio", 20);
            int id = 1;

            var existingArtist = new Artist
            {
                Id = id,
                Name = "Original Name",
                Bio = "Original Bio",
                Age = 30
            };

            var updatedArtist = new Artist
            {
                Id = id,
                Name = "Updated Name",
                Bio = "Updated Bio",
                Age = 20
            };

            _artistRepositoryMock.Setup(a => a.PutArtistAsync(It.IsAny<Artist>())).ReturnsAsync(updatedArtist);

            var result = await _artistService.PutArtistAsync(requestDto, id);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(updatedArtist, options => options.ExcludingMissingMembers());

            _artistRepositoryMock.Verify(a => a.PutArtistAsync(It.IsAny<Artist>()), Times.Once());
        }
    }
}
