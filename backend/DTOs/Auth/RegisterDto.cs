namespace backend.DTOs.Auth;

public class RegisterDto
{
    public string Nome { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}