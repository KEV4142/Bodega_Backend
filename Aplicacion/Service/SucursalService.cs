using Aplicacion.Core;
using Aplicacion.Interface;
using Microsoft.EntityFrameworkCore;
using Modelo.Entidades;
using Persistencia;

namespace Aplicacion.Service;

public class SucursalService : ISucursalService
{
    private readonly BackendContext _backendContext;

    public SucursalService(BackendContext context)
    {
        _backendContext = context;
    }
    public async Task<Result<Sucursal>> ObtenerSucursalPorIDAsync(int sucursalID)
    {
        var sucursal = await _backendContext.Sucursales!.FirstOrDefaultAsync(s => s.SucursalID == sucursalID);
        if (sucursal is null)
            return Result<Sucursal>.Failure("No se encontró la sucursal.");

        return Result<Sucursal>.Success(sucursal);
    }
}
