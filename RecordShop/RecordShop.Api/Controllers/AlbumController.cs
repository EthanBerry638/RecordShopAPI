using Microsoft.AspNetCore.Mvc;
using RecordShop.Api.Models.DTOs;
using RecordShop.Api.Services;

namespace RecordShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumController : Controller
    {
        private readonly IAlbumService _albumService;

        public AlbumController(IAlbumService albumService)
        {
            _albumService = albumService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAlbumsAsync()
        {
            var albums = await _albumService.GetAllAlbumsAsync();

            return Ok(albums);
        }

        [HttpGet("{id:min(1)}")]
        public async Task<IActionResult> GetAlbumByIdAsync(int id)
        {
            var album = await _albumService.GetAlbumByIdAsync(id);

            if (album == null)
            {
                return NotFound();
            }

            return Ok(album);
        }

        [HttpPost]
        public async Task<IActionResult> PostAlbumAsync([FromBody] PostAlbumRequest requestDTO)
        {
            var postedAlbum = await _albumService.PostAlbumAsync(requestDTO);

            return CreatedAtAction(
                nameof(GetAlbumByIdAsync),
                new { Id = postedAlbum.Id },
                postedAlbum
            );
        }

        [HttpPut("{id:min(1)}")]
        public async Task<IActionResult> PutAlbumAsync([FromBody] PutAlbumRequest requestDTO, int id)
        {
            var putAlbum = await _albumService.PutAlbumAsync(requestDTO, id);

            if (putAlbum == null)
            {
                return NotFound();
            }

            return Ok(putAlbum);
        }

        [HttpDelete("{id:min(1)}")]
        public async Task<IActionResult> DeleteAlbumByIdAsync(int id)
        {
            var deleteResult = await _albumService.DeleteAlbumByIdAsync(id);

            if (!deleteResult)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
