using Aplicacion.Tablas.Salidas.SalidaCreate;
using Aplicacion.Tablas.Salidas.SalidasResponse;

namespace Aplicacion.Interface;

public interface IDistribucionService
{
    Task<DistribucionResultado> ObtenerDistribucionAsync(List<SalidaDetRequest> detalles, CancellationToken cancellationToken);
}
