using RecordShop.Api.Models.DataModels;
using RecordShop.Api.Models.DTOs;
using RecordShop.Api.Repositories;

namespace RecordShop.Api.Services
{
    public class ArtistService : IArtistService
    {
        private readonly IArtistRepository _artistRepository;

        public ArtistService(IArtistRepository artistRepository)
        {
            _artistRepository = artistRepository;
        }

        public async Task<List<GetArtistResponse>> GetAllArtistsAsync()
        {
            var artists = await _artistRepository.GetAllArtistsAsync();

            return artists.Select(a => new GetArtistResponse
            (   a.Id,
                a.Name,
                a.Bio,
                a.Age
            )).ToList();
        }

        public async Task<GetArtistResponse?> GetArtistByIdAsync(int id)
        {
            var artist = await _artistRepository.GetArtistByIdAsync(id);

            if (artist == null)
            {
                return null;
            }

            return new GetArtistResponse
            (
                artist.Id,
                artist.Name,
                artist.Bio,
                artist.Age
            );
        }

        public async Task<PostArtistResponse> PostArtistAsync(PostArtistRequest request)
        {
            var artist = new Artist
            {
                Name = request.Name,
                Bio = request.Bio,
                Age = request.Age
            };

            var response = await _artistRepository.PostArtistAsync(artist);

            return new PostArtistResponse(response.Id, response.Name, response.Bio!, response.Age);
        }

        public async Task<PutArtistResponse?> PutArtistAsync(PutArtistRequest artist, int id)
        {
            var existingArtist = await GetArtistByIdAsync(id);

            if (existingArtist == null)
            {
                return null;
            }

            var artistToReplace = new Artist
            {
                Id = id,
                Name = artist.Name,
                Bio = artist.Bio,
                Age = artist.Age
            };

            var replacedArtist = await _artistRepository.PutArtistAsync(artistToReplace);

            return new PutArtistResponse(replacedArtist.Id, replacedArtist.Name, replacedArtist.Bio!, replacedArtist.Age);
        }
    }
}
