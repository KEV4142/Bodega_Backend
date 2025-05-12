using Modelo.Entidades;

namespace Seguridad.Interfaces;
public interface ITokenService
{
    Task<string> CreateToken(Usuario user);
}