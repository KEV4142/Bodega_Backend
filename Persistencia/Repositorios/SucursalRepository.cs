using Microsoft.EntityFrameworkCore;
using Modelo.Entidades;
using Modelo.Interfaces;

namespace Persistencia.Repositorios;

public class SucursalRepository : ISucursalRepository
{
    private readonly BackendContext _backendContext;

    public SucursalRepository(BackendContext context)
    {
        _backendContext = context;
    }
    public async Task<Sucursal?> ObtenerPorIDAsync(int sucursalID)
    {
        return await _backendContext.Sucursales!.FirstOrDefaultAsync(s => s.SucursalID == sucursalID);
    }
}