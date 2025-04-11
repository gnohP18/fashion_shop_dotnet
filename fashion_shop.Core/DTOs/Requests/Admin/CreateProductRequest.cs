namespace fashion_shop.Core.DTOs.Requests.Admin;

public class CreateProductRequest
{
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public int Price { get; set; } = default!;
    public string? ImageUrl { get; set; }
    public string Description { get; set; } = String.Empty;
    public int CategoryId { get; set; }
}