using Aplicacion.Core;
using Aplicacion.Interface;

namespace Aplicacion.Service;

public class RestriccionSalidaService : IRestriccionSalidaService
{
    private readonly ISalidaService _salidaService;

    public RestriccionSalidaService(ISalidaService salidaService)
    {
        _salidaService = salidaService;
    }
    public async Task<Result<bool>> ValidarLimiteSucursal(int sucursalID, decimal sumaDetalle, CancellationToken ct)
    {
        var total = await _salidaService.ObtenerTotalCostoPendientePorSucursal(sucursalID,ct);

        if (total > 5000)
            {return Result<bool>.Failure($" No se puede ingresar la orden de Salida dado que se ha acumulado y la Sucursal no las ha recibido. ({total}) ");}

        else if ((total + sumaDetalle) > 5000)
            {return Result<bool>.Failure($" No se puede ingresar la orden de Salida actual por sobrepasar mas de lo permitido. ( {total + sumaDetalle} ) ");}

        return Result<bool>.Success(true);
    }
}