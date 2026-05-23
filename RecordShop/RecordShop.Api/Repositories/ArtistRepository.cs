using Microsoft.EntityFrameworkCore;
using RecordShop.Api.Data;
using RecordShop.Api.Models.DataModels;

namespace RecordShop.Api.Repositories
{
    public class ArtistRepository : IArtistRepository
    {
        private readonly RecordShopContext _db;

        public ArtistRepository(RecordShopContext db)
        {
            _db = db;
        }

        public async Task<List<Artist>> GetAllArtistsAsync()
        {
            return await _db.Artists.ToListAsync();
        }

        public async Task<Artist?> GetArtistByIdAsync(int id)
        {
            return await _db.Artists.FindAsync(id);
        }

        public async Task<Artist> PostArtistAsync(Artist artist)
        {
            await _db.Artists.AddAsync(artist);
            await _db.SaveChangesAsync(); 
            return artist;
        }

        public async Task<Album> PutArtistAsync(Artist artist)
        {
            return null;
        }
    }
}
