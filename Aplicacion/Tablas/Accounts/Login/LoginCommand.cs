using Aplicacion.Core;
using Seguridad.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Modelo.Entidades;
using Modelo.Custom;


namespace Aplicacion.Tablas.Accounts.Login;
public class LoginCommand
{
    public record LoginCommandRequest(LoginRequest loginRequest) : IRequest<Result<Profile>>;

    internal class LoginCommandHandler
        : IRequestHandler<LoginCommandRequest, Result<Profile>>
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(
            UserManager<Usuario> userManager,
            ITokenService tokenService
        )
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<Result<Profile>> Handle(
            LoginCommandRequest request,
            CancellationToken cancellationToken
        )
        {
            var user = await _userManager.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(x => x.Email == request.loginRequest.Email);

            if (user is null)
            {
                return Result<Profile>.Failure("No se encontro el usuario");
            }

            var resultado = await _userManager
            .CheckPasswordAsync(user, request.loginRequest.Password!);

            if (!resultado)
            {
                return Result<Profile>.Failure("Las credenciales son incorrectas");
            }
            var tipo="";
            /* var roleNames = await _userManager.GetRolesAsync(user);
            if (roleNames.Contains(CustomRoles.ADMINBODEGA))
            {
                tipo="Administrador";
            }
            else if (roleNames.Contains(CustomRoles.CLIENT))
            {
                tipo="Operador";
            }
            else
            {
                tipo="Sin Asignacion de Rol.";
            } */
            tipo = user.Role?.Name switch
            {
                CustomRoles.ADMINBODEGA => "Administrador",
                CustomRoles.CLIENT => "Operador",
                _ => "Sin Asignacion de Rol."
            };

            var profile = new Profile
            {
                Email = user.Email,
                NombreCompleto = user.NombreCompleto,
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                Tipo=tipo
            };

            return Result<Profile>.Success(profile);
        }
    }
    public class LoginCommandRequestValidator : AbstractValidator<LoginCommandRequest>
    {
        public LoginCommandRequestValidator()
        {
            RuleFor(x => x.loginRequest).SetValidator(new LoginValidator());
        }
    }
}
