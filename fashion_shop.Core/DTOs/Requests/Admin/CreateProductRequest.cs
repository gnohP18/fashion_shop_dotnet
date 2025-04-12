using System.ComponentModel.DataAnnotations;

namespace fashion_shop.Core.DTOs.Requests.Admin;

public class CreateProductRequest
{
    [Required(ErrorMessage = "Name is required")]
    [MinLength(3, ErrorMessage = "Name must be at least 3 characters")]
    public string Name { get; set; } = default!;

    [Required(ErrorMessage = "Slug is required")]
    [MinLength(3, ErrorMessage = "Slug must be at least 3 characters")]
    public string Slug { get; set; } = default!;

    [Required(ErrorMessage = "Slug is required")]
    public int Price { get; set; }

    public string? ImageUrl { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
}