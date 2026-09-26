using backend.Data;
using backend.DTOs.Auth;
using backend.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
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

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return Unauthorized();
        }

        var utilizador = await _context.Utilizadores
            .FindAsync(int.Parse(userId));

        if (utilizador == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            utilizador.Id,
            utilizador.Nome,
            utilizador.Username
        });
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

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, utilizador.Id.ToString()),
            new Claim(ClaimTypes.Name, utilizador.Username)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]!
            )
        );

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new
        {
            token = tokenString
        });
            }
}