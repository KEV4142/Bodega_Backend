using Aplicacion.Interface;
using Aplicacion.Tablas.Lotes.DTOLotes;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Service;

public class LoteService : ILoteService
{
    private readonly BackendContext _backendContext;
    private readonly IMapper _mapper;

    public LoteService(BackendContext context, IMapper mapper)
    {
        _backendContext = context;
        _mapper = mapper;
    }
    public async Task<List<LoteCompletoResponse>> ObtenerLotesDisponiblesOrdenados(int productoID)
    {
        return await _backendContext.Lotes!
            .Where(lote => lote.ProductoID == productoID && lote.Cantidad > 0)
            .OrderBy(l => l.FechaVencimiento)
            .ProjectTo<LoteCompletoResponse>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }
}
