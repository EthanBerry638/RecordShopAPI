using Microsoft.AspNetCore.Mvc;
using RecordShop.Api.Models.DTOs;
using RecordShop.Api.Services;

namespace RecordShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistController : Controller
    {
        private readonly IArtistService _artistService;

        public ArtistController(IArtistService artistService)
        {
            _artistService = artistService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllArtistsAsync()
        {
            var artists = await _artistService.GetAllArtistsAsync();

            return Ok(artists);
        }

        [HttpGet("{id:min(1)}")]
        public async Task<IActionResult> GetArtistByIdAsync(int id)
        {
            var artist = await _artistService.GetArtistByIdAsync(id);

            if (artist == null)
            {
                return NotFound();
            }

            return Ok(artist);
        }

        [HttpPost]
        public async Task<IActionResult> PostArtistAsync([FromBody]PostArtistRequest request)
        {
            var artist = await _artistService.PostArtistAsync(request);

            return CreatedAtAction(
                nameof(GetArtistByIdAsync),
                new { Id = artist.Id },
                artist
            );
        }

        [HttpPut]
        public async Task<IActionResult> PutArtistAsync([FromBody] PutArtistRequest request, int id)
        {
            return BadRequest();
        }
    }
}
