using Aplicacion.Interface;
using Aplicacion.Tablas.Lotes.DTOLotes;
using Aplicacion.Tablas.Salidas.SalidasResponse;
using AutoMapper;
using Modelo.Entidades;
using Modelo.Interfaces;


namespace Aplicacion.Service;

public class LoteService : ILoteService
{
    private readonly ILoteRepository _loteRepository;
    private readonly IMapper _mapper;

    public LoteService(ILoteRepository loteRepository, IMapper mapper)
    {
        _loteRepository = loteRepository;
        _mapper = mapper;
    }
    public async Task<List<LoteCompletoResponse>> ObtenerLotesDisponiblesOrdenados(int productoID, CancellationToken cancellationToken)
    {
        var lotes = await _loteRepository.ObtenerLotesDisponiblesOrdenadosAsync(productoID, cancellationToken);
        return _mapper.Map<List<LoteCompletoResponse>>(lotes);
    }

    public async Task<List<LoteCantidadListado>> ObtenerLotesDisponiblesParaProducto(int productoID, CancellationToken cancellationToken)
    {
        var lotes = await _loteRepository.ObtenerLotesDisponiblesParaProductoAsync(productoID, cancellationToken);
        return _mapper.Map<List<LoteCantidadListado>>(lotes);
    }

    public async Task<List<Lote>> ObtenerLotesPorIDLista(List<int> loteIds, CancellationToken cancellationToken)
    {
        var lotes = await _loteRepository.ObtenerLotesPorIDListaAsync(loteIds,cancellationToken);
        return lotes;
    }
}
