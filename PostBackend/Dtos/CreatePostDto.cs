using System.ComponentModel.DataAnnotations;

namespace PostBackend.Dtos;

public class CreatePostDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    // Optional image URL (you can also upload via separate endpoint)
    public string? ImageUrl { get; set; }
}
