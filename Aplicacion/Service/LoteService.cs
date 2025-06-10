using Aplicacion.Interface;
using Aplicacion.Tablas.Lotes.DTOLotes;
using AutoMapper;
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
    public async Task<List<LoteCompletoResponse>> ObtenerLotesDisponiblesOrdenados(int productoID)
    {
        var lotes = await _loteRepository.ObtenerLotesDisponiblesOrdenadosAsync(productoID);
        return _mapper.Map<List<LoteCompletoResponse>>(lotes);
    }
}
