using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RecordShop.Api.Data;
using RecordShop.Api.Models.DataModels;
using RecordShop.Api.Repositories;
using System.Text.Json;

namespace RecordShop.Tests.Unit.RepositoryTests
{
    public class ArtistRepositoryTests
    {
        private ArtistRepository _artistRepository;
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

            _artistRepository = new ArtistRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
            _connection.Dispose();
        }

        [Test]
        public async Task GetAllArtistsAsync_ShouldReturnEmptyList_WhenArtistTableIsEmpty()
        {
            var expectedList = new List<Artist>();

            _context.Artists.RemoveRange(_context.Artists);
            await _context.SaveChangesAsync();

            var result = await _artistRepository.GetAllArtistsAsync();

            result.Should().BeEquivalentTo(expectedList);
        }

        [Test]
        public async Task GetAllArtistsAsync_ShouldReturnListOfArtists_WhenDatabaseIsSeeded()
        {
            var jsonString = File.ReadAllText("Resources\\artists.json");
            var expectedArtists = JsonSerializer.Deserialize<List<Artist>>(jsonString);

            var result = await _artistRepository.GetAllArtistsAsync();

            result.Should().BeEquivalentTo(expectedArtists, options => options.ExcludingMembersNamed("Id"));
        }

        [Test]
        public async Task GetArtistByIdAsync_ShouldReturnNull_WhenArtistDoesNotExist()
        {
            int id = 100000;

            var result = await _artistRepository.GetArtistByIdAsync(id);

            result.Should().BeNull();
        }

        [Test]
        public async Task GetArtistByIdAsync_ShouldReturnArtist_WhenArtistDoesExist()
        {
            int id = 1;

            var expectedArtist = new Artist 
            { 
                Id = id,
                Name = "Michael Jackson", 
                Bio = "An American singer, songwriter, and dancer. Dubbed the 'King of Pop', he is regarded as one of the most significant cultural figures of the 20th century.",
                Age = 50 
            };

            var result = await _artistRepository.GetArtistByIdAsync(id);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedArtist);
        }

        [Test]
        public async Task PostArtistAsync_ShouldReturnArtist_WhenArtistIsValid()
        {
            var expectedArtist = new Artist
            {
                Name = "Test",
                Bio = "Test",
                Age = 28
            };

            var result = await _artistRepository.PostArtistAsync(expectedArtist);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedArtist);
            result.Id.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task PutArtistAsync_ShouldReturnArtist_WhenArtistIsValid()
        {
            var originalArtist = new Artist
            {
                Name = "Test",
                Bio = "Test",
                Age = 28
            };

            var seededArtist = await _artistRepository.PostArtistAsync(originalArtist);
            var id = seededArtist.Id;

            seededArtist.Name = "Updated Test";
            seededArtist.Bio = "Updated Test";
            seededArtist.Age = 30;

            var result = await _artistRepository.PutArtistAsync(seededArtist);

            result.Should().NotBeNull();
            result.Id.Should().Be(seededArtist.Id);
            result.Name.Should().Be(seededArtist.Name);
            result.Bio.Should().Be(seededArtist.Bio);
            result.Age.Should().Be(seededArtist.Age);

            _context.ChangeTracker.Clear();

            var inDb = await _artistRepository.GetArtistByIdAsync(result.Id);

            inDb.Should().NotBeNull();
            inDb.Should().BeEquivalentTo(result);
        }
    }
}
