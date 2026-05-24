using System.ComponentModel.DataAnnotations;

namespace RecordShop.Api.Models.DTOs
{
    public record PostArtistRequest(
        [Required][MaxLength(150, ErrorMessage = "Name cannot exceed 150 characters")] string Name,
        [MaxLength(500, ErrorMessage = "Bio cannot exceed 500 characters")] string Bio,
        [Required][Range(1, 120, ErrorMessage = "Age must be between 1 and 120")] int Age
    );

    public record PostArtistResponse(
        int Id,
        string Name,
        string Bio,
        int Age
    );
}
