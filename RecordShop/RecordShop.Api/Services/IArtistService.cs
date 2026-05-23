using RecordShop.Api.Models.DataModels;
using RecordShop.Api.Models.DTOs;

namespace RecordShop.Api.Services
{
    public interface IArtistService
    {
        Task<List<GetArtistResponse>> GetAllArtistsAsync();
        Task<GetArtistResponse?> GetArtistByIdAsync(int id);
        Task<PostArtistResponse> PostArtistAsync(PostArtistRequest request);
        Task<PutArtistResponse?> PutArtistAsync(PutArtistRequest artist, int id);
    }
}
