using Aplicacion.Core;
using Aplicacion.Interface;
using Aplicacion.Tablas.Sucursales.DTOSucursales;
using MediatR;

namespace Aplicacion.Tablas.Sucursales.GetSucursalesActivas;

public class GetSucursalesActivas
{
    public record GetSucursalesActivasQueryRequest : IRequest<Result<List<SucursalResponse>>>
    {
    }
    internal class GetSucursalesActivasQueryHandler
        : IRequestHandler<GetSucursalesActivasQueryRequest, Result<List<SucursalResponse>>>
    {
        private readonly ISucursalService _sucursalService;

        public GetSucursalesActivasQueryHandler(ISucursalService sucursalService)
        {
            _sucursalService = sucursalService;
        }

        public async Task<Result<List<SucursalResponse>>> Handle(
            GetSucursalesActivasQueryRequest request,
            CancellationToken cancellationToken
        )
        {
            var listaSucursales = await _sucursalService.ObtenerSucursalesActivas(cancellationToken);

            return listaSucursales;
        }
    }
}