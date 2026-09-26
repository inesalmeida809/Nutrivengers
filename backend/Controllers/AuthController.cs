using backend.Data;
using backend.DTOs.Auth;
using backend.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        // Verificar se o username já existe
        var utilizadorExiste = await _context.Utilizadores
            .AnyAsync(u => u.Username == dto.Username);

        if (utilizadorExiste)
        {
            return BadRequest("Username já está a ser utilizado.");
        }

        // Criar hash da password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        // Criar utilizador
        var utilizador = new Utilizador
        {
            Nome = dto.Nome,
            Username = dto.Username,
            PasswordHash = passwordHash
        };

        _context.Utilizadores.Add(utilizador);
        await _context.SaveChangesAsync();

        return Ok("Utilizador registado com sucesso.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var utilizador = await _context.Utilizadores
            .FirstOrDefaultAsync(u => u.Username == dto.Username);

        if (utilizador == null)
        {
            return Unauthorized("Username ou password incorretos.");
        }

        var passwordValida = BCrypt.Net.BCrypt.Verify(
            dto.Password,
            utilizador.PasswordHash
        );

        if (!passwordValida)
        {
            return Unauthorized("Username ou password incorretos.");
        }

        return Ok("Login efetuado com sucesso.");
    }
}