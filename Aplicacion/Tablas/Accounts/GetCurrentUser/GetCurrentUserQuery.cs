using System.Net;
using Aplicacion.Core;
using Aplicacion.Interface;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Modelo.Entidades;

namespace Aplicacion.Tablas.Accounts.GetCurrentUser;
public class GetCurrentUserQuery
{
    public record GetCurrentUserQueryRequest(GetCurrentUserRequest getCurrentUserRequest) : IRequest<Result<Profile>>;

    internal class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQueryRequest, Result<Profile>>
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly IProfileFactory _profileFactory;

        public GetCurrentUserQueryHandler(UserManager<Usuario> userManager, IProfileFactory profileFactory)
        {
            _userManager = userManager;
            _profileFactory = profileFactory;
        }

        public async Task<Result<Profile>> Handle(
            GetCurrentUserQueryRequest request,
            CancellationToken cancellationToken
            )
        {
            var user = await _userManager.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(x => x.Email == request.getCurrentUserRequest.Email);

            if (user is null)
            {
                return Result<Profile>.Failure("No se encontro el usuario.", HttpStatusCode.NotFound);
            }
            var profile = await _profileFactory.CrearAsync(user);

            return Result<Profile>.Success(profile);
        }
    }



}