using Modelo.Entidades;

namespace Modelo.Interfaces;

public interface ISalidaEncRepository
{
    Task<SalidaEnc?> ObtenerPorIDAsync(int salidaID, CancellationToken cancellationToken);
    Task<bool> ActualizarEstadoAsync(SalidaEnc salida, string nuevoEstado, string usuarioRecibeID, CancellationToken cancellationToken);
    Task<bool> InsertarAsync(SalidaEnc salidaEnc, CancellationToken cancellationToken);
    IQueryable<SalidaEnc> GetQueryable();
    Task<decimal> ObtenerTotalCostoPendientePorSucursalAsync(int sucursalID, CancellationToken cancellationToken);
}
