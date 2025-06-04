using Aplicacion.Interface;
using Aplicacion.Tablas.Accounts;
using Modelo.Custom;
using Modelo.Entidades;
using Seguridad.Interfaces;

namespace Aplicacion.Service;

public class ProfileFactory : IProfileFactory
{
    private readonly ITokenService _tokenService;
    private readonly IRoleTranslator _roleTranslator;

    public ProfileFactory(ITokenService tokenService, IRoleTranslator roleTranslator)
    {
        _tokenService = tokenService;
        _roleTranslator = roleTranslator;
    }
    public async Task<Profile> CrearAsync(Usuario usuario)
    {
        var tipo = _roleTranslator.ObtenerRol(usuario.Role?.Name);

        return new Profile
        {
            Email = usuario.Email,
            NombreCompleto = usuario.NombreCompleto,
            Username = usuario.UserName,
            Token = await _tokenService.CreateToken(usuario),
            Tipo = tipo
        };
    }
}
