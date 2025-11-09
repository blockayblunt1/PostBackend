using System.ComponentModel.DataAnnotations;

namespace PostBackend.Dtos;

public class UpdatePostDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }
}
