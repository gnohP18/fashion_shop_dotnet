namespace fashion_shop.Core.DTOs.Requests.Admin;

public class CreateUserByAdminRequest
{
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}