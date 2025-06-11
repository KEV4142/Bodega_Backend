using Aplicacion.Core;
using Aplicacion.Interface;
using Aplicacion.Tablas.Sucursales.DTOSucursales;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public GetSucursalesActivasQueryHandler(ISucursalService sucursalService, IMapper mapper)
        {
            _sucursalService = sucursalService;
            _mapper = mapper;
        }

        public async Task<Result<List<SucursalResponse>>> Handle(
            GetSucursalesActivasQueryRequest request,
            CancellationToken cancellationToken
        )
        {
            var listaSucursales = await _sucursalService.ObtenerSucursalesActivas(cancellationToken);
            var sucursales = _mapper.Map<List<SucursalResponse>>(listaSucursales);

            return Result<List<SucursalResponse>>.Success(sucursales);
        }
    }
}