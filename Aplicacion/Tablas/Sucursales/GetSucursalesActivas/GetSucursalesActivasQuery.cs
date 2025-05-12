using Aplicacion.Core;
using Aplicacion.Tablas.Sucursales.DTOSucursales;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Tablas.Sucursales.GetSucursalesActivas;
public class GetSucursalesActivas
{
    public record GetSucursalesActivasQueryRequest : IRequest<Result<List<SucursalResponse>>>
    {
    }
    internal class GetSucursalesActivasQueryHandler
        : IRequestHandler<GetSucursalesActivasQueryRequest, Result<List<SucursalResponse>>>
    {
        private readonly BackendContext _context;
        private readonly IMapper _mapper;

        public GetSucursalesActivasQueryHandler(BackendContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<List<SucursalResponse>>> Handle(
            GetSucursalesActivasQueryRequest request, 
            CancellationToken cancellationToken
        )
        {
            var sucursales = await _context.Sucursales!
                .Where(s => s.Estado!=null && s.Estado.ToUpper() =="A")
                .ProjectTo<SucursalResponse>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Result<List<SucursalResponse>>.Success(sucursales);
        }
    }
}