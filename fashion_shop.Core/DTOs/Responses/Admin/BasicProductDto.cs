namespace fashion_shop.Core.DTOs.Responses.Admin;

public class BasicProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public int Price { get; set; }
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = default!;
    public bool IsVariant { get; set; }
}