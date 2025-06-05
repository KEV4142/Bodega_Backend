using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Modelo.Entidades;
using Seguridad.Interfaces;

namespace Seguridad.Security;
public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<Usuario> _userManager;

    public TokenService(IConfiguration configuration, UserManager<Usuario> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
    }

    public Task<string> CreateToken(Usuario usuario)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.UserName!),
            new Claim(ClaimTypes.NameIdentifier, usuario.Id),
            new Claim(ClaimTypes.Email, usuario.Email!)
        };

        if (usuario.Role?.Name is not null)
        {
            claims.Add(new Claim(ClaimTypes.Role, usuario.Role.Name));
        }
        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["TokenKey"]!)),
            SecurityAlgorithms.HmacSha256
        );

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return Task.FromResult(tokenHandler.WriteToken(token));
    }

}