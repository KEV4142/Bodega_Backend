using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modelo.Entidades;
using Modelo.Interfaces;

namespace Persistencia.Repositorios;

public class SalidaEncRepository : ISalidaEncRepository
{
    private readonly BackendContext _backendContext;
    private readonly ILogger<SalidaEncRepository> _logger;
    private readonly bool _soportaTransacciones;

    public SalidaEncRepository(BackendContext context, ILogger<SalidaEncRepository> logger, bool soportaTransacciones = true)
    {
        _backendContext = context;
        _logger = logger;
        _soportaTransacciones = soportaTransacciones;
    }
    public async Task<SalidaEnc?> ObtenerPorIDAsync(int salidaID, CancellationToken cancellationToken)
    {
        return await _backendContext.SalidaEncs!.FirstOrDefaultAsync(se => se.SalidaID == salidaID, cancellationToken);
    }
    public async Task<bool> ActualizarEstadoAsync(SalidaEnc salida, string nuevoEstado, string usuarioRecibeID, CancellationToken cancellationToken)
    {
        salida.Estado = nuevoEstado.ToUpper();
        salida.FechaRecibido = DateTime.Now;
        salida.UsuarioRecibe = usuarioRecibeID;

        try
        {
            if (!_soportaTransacciones)
            {
                await _backendContext.SaveChangesAsync(cancellationToken);
                return true;
            }

            await using var transaction = await _backendContext.Database.BeginTransactionAsync(cancellationToken);
            await _backendContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }
    public IQueryable<SalidaEnc> GetQueryable()
    {
        return _backendContext.SalidaEncs!.AsQueryable();
    }
    public async Task<bool> InsertarAsync(SalidaEnc salidaEnc, CancellationToken cancellationToken)
    {
        try
        {
            if (!_soportaTransacciones)
            {
                _backendContext.Add(salidaEnc);
                await _backendContext.SaveChangesAsync(cancellationToken);
                return true;
            }

            await using var transaction = await _backendContext.Database.BeginTransactionAsync(cancellationToken);

            _backendContext.Add(salidaEnc);
            await _backendContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            var loteIDS = string.Join(",", salidaEnc.SalidaDets.Select(detalle => detalle.LoteID));
            var productoIDS = string.Join(",", salidaEnc.SalidaDets.Select(detalle => detalle.Lote.ProductoID));
            var cantidades = string.Join(",", salidaEnc.SalidaDets.Select(detalle => detalle.Cantidad));

            _logger.LogError(ex, "Error al guardar la salida (SucursalID: {SucursalID}, UsuarioID: {UsuarioID}, LoteID's: {LoteIDS}, ProductoID's: {productoIDS}, Cantidad: {cantidades}) en SalidaEncCreateCommand",
                salidaEnc.SucursalID,
                salidaEnc.UsuarioID,
                loteIDS,
                productoIDS,
                cantidades);
            return false;
        }
    }
    public async Task<decimal> ObtenerTotalCostoPendientePorSucursalAsync(int sucursalID, CancellationToken ct)
    {
        /* return await _backendContext.SalidaEncs
            .Where(se => se.SucursalID == sucursalID && se.Estado == "E")
            .SelectMany(se => se.SalidaDets)
            .Where(sd => sd.Lote != null)
            .SumAsync(sd => (decimal?)(sd.Lote.Costo * sd.Cantidad), ct) ?? 0m; */
        return await _backendContext.SalidaEncs
            .Where(se => se.SucursalID == sucursalID && se.Estado == "E")
            .SelectMany(se => se.SalidaDets)
            .Where(sd => sd.Lote != null)
            .SumAsync(sd => (decimal?)(sd.Costo * sd.Cantidad), ct) ?? 0m;
    }

}
