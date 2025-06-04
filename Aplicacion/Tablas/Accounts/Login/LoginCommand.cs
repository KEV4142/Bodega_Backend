using Aplicacion.Core;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Modelo.Entidades;
using Aplicacion.Interface;


namespace Aplicacion.Tablas.Accounts.Login;
public class LoginCommand
{
    public record LoginCommandRequest(LoginRequest loginRequest) : IRequest<Result<Profile>>;

    internal class LoginCommandHandler
        : IRequestHandler<LoginCommandRequest, Result<Profile>>
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly IProfileFactory _profileFactory;

        public LoginCommandHandler(
            UserManager<Usuario> userManager,
            IProfileFactory profileFactory
        )
        {
            _userManager = userManager;
            _profileFactory = profileFactory;
        }

        public async Task<Result<Profile>> Handle(
            LoginCommandRequest request,
            CancellationToken cancellationToken
        )
        {
            var user = await _userManager.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(x => x.Email!.ToLower() == request.loginRequest.Email!.ToLower());

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

            var profile = await _profileFactory.CrearAsync(user);

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
