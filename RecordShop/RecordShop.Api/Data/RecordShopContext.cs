using Microsoft.EntityFrameworkCore;
using RecordShop.Api.Models.DataModels;
using System.Text.Json;

namespace RecordShop.Api.Data
{
    public class RecordShopContext : DbContext
    {
        private readonly string _albumFilePath = "Resources\\albums.json";
        private readonly string _artistFilePath = "Resources\\artists.json";
        public DbSet<Album> Albums { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public RecordShopContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Album>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Title)
                    .IsRequired()
                    .HasMaxLength(150);
                entity.Property(a => a.Description)
                    .IsRequired()
                    .HasMaxLength(250);
                entity.Property(a => a.ReleaseDate);
                entity.Property(a => a.Price)
                    .IsRequired()
                    .HasPrecision(9, 2);

                if (!Database.IsSqlite())
                {
                    entity.ToTable(t => t.HasCheckConstraint("CK_Price_MaxLimit", "[Price] <= 2000000.00"));
                }
            });

            modelBuilder.Entity<Artist>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Name)
                    .IsRequired()
                    .HasMaxLength(150);
                entity.Property(a => a.Bio)
                    .HasMaxLength(500);
                entity.Property(a => a.Age)
                    .IsRequired();

                if (!Database.IsSqlite())
                {
                    entity.ToTable(t => t.HasCheckConstraint("CK_Artist_Age_Range", "[Age] >=1 AND [Age] <= 120"));
                }
            });
        }

        public void SeedData()
        {
            if (!Albums.Any())
            {
                var jsonAlbums = File.ReadAllText(_albumFilePath);
                var albums = JsonSerializer.Deserialize<List<Album>>(jsonAlbums);

                if (albums != null)
                {
                    Albums.AddRange(albums);
                    SaveChanges();
                }
            }
            if (!Artists.Any())
            {
                var jsonArtists = File.ReadAllText(_artistFilePath);
                var artists = JsonSerializer.Deserialize<List<Artist>>(jsonArtists);

                if (artists != null)
                {
                    Artists.AddRange(artists);
                    SaveChanges();
                }
            }
        }
    }
}
