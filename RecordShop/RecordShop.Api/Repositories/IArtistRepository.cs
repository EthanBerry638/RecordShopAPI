using RecordShop.Api.Models.DataModels;

namespace RecordShop.Api.Repositories
{
    public interface IArtistRepository
    {
        Task<List<Artist>> GetAllArtistsAsync();
        Task<Artist?> GetArtistByIdAsync(int id);
        Task<Artist> PostArtistAsync(Artist artist);
        Task<Artist> PutArtistAsync(Artist artist);
        Task<bool> DeleteArtistByIdAsync(int id);
    }
}
