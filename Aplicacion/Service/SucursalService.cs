using Aplicacion.Core;
using Aplicacion.Interface;
using Modelo.Entidades;
using Modelo.Interfaces;

namespace Aplicacion.Service;

public class SucursalService : ISucursalService
{
    private readonly ISucursalRepository _sucursalRepository;

    public SucursalService(ISucursalRepository sucursalRepository)
    {
        _sucursalRepository = sucursalRepository;
    }
    public async Task<Result<Sucursal>> ObtenerSucursalPorID(int sucursalID)
    {
        var sucursal = await _sucursalRepository.ObtenerPorIDAsync(sucursalID);
        if (sucursal is null)
            return Result<Sucursal>.Failure("No se encontró la sucursal.");

        return Result<Sucursal>.Success(sucursal);
    }
}
