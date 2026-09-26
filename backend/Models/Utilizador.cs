namespace backend.Models;

public class Utilizador
{
    public int Id { get; set;}

    public string Nome { get; set;} = string.Empty;

    public string Username { get; set;} = string.Empty;

    public string PasswordHash { get; set;} = string.Empty;
}