namespace RecordShop.Api.Models.DTOs
{
    public record GetArtistResponse
    (
        int Id,
        string Name,
        string? Bio,
        int Age
    )
    { }
}
