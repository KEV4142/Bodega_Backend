using Aplicacion.Core;
using Modelo.Entidades;

namespace Aplicacion.Interface;

public interface ISalidaService
{
    Task<Result<SalidaEnc>> ObtenerSalidaPorID(int salidaID, CancellationToken cancellationToken);
    Task<Result<int>> CambiarEstadoSalida(SalidaEnc salida, string nuevoEstado, string usuarioID, CancellationToken cancellationToken);
    Task<Result<int>> RegistrarSalida(SalidaEnc salidaEnc, CancellationToken cancellationToken);
    IQueryable<SalidaEnc> GetQueryable();
}
