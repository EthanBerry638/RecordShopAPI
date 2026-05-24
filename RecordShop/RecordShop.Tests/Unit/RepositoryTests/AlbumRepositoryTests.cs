using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RecordShop.Api.Data;
using RecordShop.Api.Models.DataModels;
using RecordShop.Api.Repositories;
using System.Text.Json;

namespace RecordShop.Tests.Unit.RepositoryTests
{
    public class AlbumRepositoryTests
    {
        private AlbumRepository _albumRepository;
        private RecordShopContext _context;
        private SqliteConnection _connection;

        [SetUp]
        public void Setup()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<RecordShopContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new RecordShopContext(options);

            _context.Database.EnsureCreated();

            _context.SeedData();

            _albumRepository = new AlbumRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
            _connection.Dispose();
        }

        [Test]
        public async Task GetAllAlbumsAsync_ShouldReturnListOfAlbums_WhenDatabaseIsSeeded()
        {
            var jsonString = File.ReadAllText("albums.json");
            var expectedAlbums = JsonSerializer.Deserialize<List<Album>>(jsonString);

            var result = await _albumRepository.GetAllAlbumsAsync();

            result.Should().BeEquivalentTo(expectedAlbums, options => options.ExcludingMembersNamed("Id"));
        }

        [Test]
        public async Task GetAllAlbumsAsync_ShouldReturnEmptyList_WhenDatabaseIsEmpty()
        {
            var testList = new List<Album>();

            _context.Albums.RemoveRange(_context.Albums);
            await _context.SaveChangesAsync();

            var result = await _albumRepository.GetAllAlbumsAsync();

            result.Should().BeEquivalentTo(testList);
        }

        [Test]
        public void GetAllAlbumsAsync_ShouldThrowException_WhenDatabaseIsDown()
        {
            _connection.Close();

            Func<Task> act = async () => await _albumRepository.GetAllAlbumsAsync();

            act.Should().ThrowAsync<SqliteException>();
        }

        [Test]
        public async Task GetAlbumByIdAsync_ShouldReturnNull_WhenDatabaseDoesNotContainAlbumWithSpecifiedId()
        {
            int testId = 1000000;

            var result = await _albumRepository.GetAlbumByIdAsync(testId);

            result.Should().BeNull();
        }

        [Test]
        public async Task GetAlbumByIdAsync_ShouldReturnAlbum_WhenDatabaseDoesAlbumWithSpecifiedId()
        {
            var testAlbum = new Album
            {
                Id = 3,
                Title = "The Dark Side of the Moon",
                Description = "A progressive rock concept album by Pink Floyd exploring themes of time, greed, and conflict.",
                ReleaseDate = new DateOnly(1973, 03, 01),
                Price = 15.00M
            };
            int testId = 3;

            var result = await _albumRepository.GetAlbumByIdAsync(testId);

            result.Should().BeOfType<Album>();
            result.Should().BeEquivalentTo(testAlbum);
        }

        [Test]
        public async Task PostAlbumAsync_ShouldReturnAlbum_WhenAlbumIsValid()
        {
            var testAlbum = new Album
            {
                Title = "Brand New Album",
                Description = "An Album That Is Brand New",
                ReleaseDate = new DateOnly(1929, 09, 26),
                Price = 14030.21M
            };

            var result = await _albumRepository.PostAlbumAsync(testAlbum);

            result.Id.Should().NotBe(0);
            result.Should().NotBeNull();
        }

        [Test]
        public async Task PutAlbumAsync_ShouldReturnAlbum_WhenAlbumIsValid()
        {
            var originalAlbum = new Album
            {
                Title = "The Dark Side of the Moon",
                Description = "A progressive rock concept album by Pink Floyd exploring themes of time, greed, and conflict.",
                ReleaseDate = new DateOnly(1973, 03, 01),
                Price = 15.00M
            };

            var seededAlbum = await _albumRepository.PostAlbumAsync(originalAlbum);
            var id = seededAlbum.Id;

            seededAlbum.Title = "Updated Title";
            seededAlbum.Description = "Updated Desc";
            seededAlbum.ReleaseDate = null;
            seededAlbum.Price = 12.99M;

            var result = await _albumRepository.PutAlbumAsync(seededAlbum);

            result.Should().NotBeNull();
            result.Title.Should().Be("Updated Title");
            result.Description.Should().Be("Updated Desc");
            result.ReleaseDate.Should().BeNull();
            result.Price.Should().Be(12.99M);

            var inDb = await _albumRepository.GetAlbumByIdAsync(id);
            inDb!.Title.Should().Be("Updated Title");
        }

        [Test]
        public async Task DeleteAlbumByIdAsync_ShouldReturnTrue_WhenAlbumIsDeleted()
        {
            int existingId = 1;

            var result = await _albumRepository.DeleteAlbumByIdAsync(existingId);

            result.Should().BeTrue();

            var deletedAlbum = await _context.Albums.FindAsync(existingId);
            deletedAlbum.Should().BeNull();
        }

        [Test]
        public async Task DeleteAlbumByIdAsync_ShouldReturnFalse_WhenAlbumIsNotFound()
        {
            int existingId = 500000;

            var result = await _albumRepository.DeleteAlbumByIdAsync(existingId);

            result.Should().BeFalse();
        }
    }
}
