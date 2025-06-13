using Aplicacion.Core;
using Aplicacion.Interface;
using Aplicacion.Tablas.Sucursales.DTOSucursales;
using AutoMapper;
using Modelo.Entidades;
using Modelo.Interfaces;

namespace Aplicacion.Service;

public class SucursalService : ISucursalService
{
    private readonly ISucursalRepository _sucursalRepository;
    private readonly IMapper _mapper;

    public SucursalService(ISucursalRepository sucursalRepository, IMapper mapper)
    {
        _sucursalRepository = sucursalRepository;
        _mapper = mapper;
    }
    public async Task<Result<Sucursal>> ObtenerSucursalPorID(int sucursalID, CancellationToken cancellationToken)
    {
        var sucursal = await _sucursalRepository.ObtenerPorIDAsync(sucursalID, cancellationToken);
        if (sucursal is null)
            return Result<Sucursal>.Failure("No se encontró la sucursal.");

        return Result<Sucursal>.Success(sucursal);
    }
    public async Task<Result<SucursalResponse>> ObtenerSucursalPorIDResponse(int sucursalID, CancellationToken cancellationToken)
    {
        var sucursal = await _sucursalRepository.ObtenerPorIDAsync(sucursalID, cancellationToken);
        if (sucursal is null)
            return Result<SucursalResponse>.Failure("No se encontró la sucursal.");
        var sucursalDTO = _mapper.Map<SucursalResponse>(sucursal);

        return Result<SucursalResponse>.Success(sucursalDTO);
    }
    public async Task<Result<List<SucursalResponse>>> ObtenerSucursalesActivas(CancellationToken cancellationToken)
    {
        var sucursales = await _sucursalRepository.ObtenerSucursalesActivasAsync(cancellationToken);
        var sucursalesDTO = _mapper.Map<List<SucursalResponse>>(sucursales);

        return Result<List<SucursalResponse>>.Success(sucursalesDTO);
    }

}
