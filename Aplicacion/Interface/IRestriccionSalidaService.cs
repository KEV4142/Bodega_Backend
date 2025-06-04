using Aplicacion.Core;

namespace Aplicacion.Interface;
    public interface IRestriccionSalidaService
    {
        Task<Result<bool>> ValidarLimiteSucursal(int sucursalId, decimal nuevaSuma, CancellationToken ct);
    }