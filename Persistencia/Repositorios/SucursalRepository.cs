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
    public async Task<Sucursal?> ObtenerPorIDAsync(int sucursalID, CancellationToken cancellationToken)
    {
        return await _backendContext.Sucursales!.FirstOrDefaultAsync(s => s.SucursalID == sucursalID, cancellationToken);
    }
    public async Task<List<Sucursal>> ObtenerSucursalesActivasAsync(CancellationToken cancellationToken)
    {
        return await _backendContext.Sucursales!
            .Where(s => s.Estado != null && s.Estado.ToUpper().Equals("A"))
            .ToListAsync(cancellationToken);
    }

}