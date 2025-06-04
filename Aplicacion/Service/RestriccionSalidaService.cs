using Aplicacion.Core;
using Aplicacion.Interface;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Service;

public class RestriccionSalidaService : IRestriccionSalidaService
{
    private readonly BackendContext _backendContext;

    public RestriccionSalidaService(BackendContext context)
    {
        _backendContext = context;
    }
    public async Task<Result<bool>> ValidarLimiteSucursal(int sucursalID, decimal sumaDetalle, CancellationToken ct)
    {
        var total = await _backendContext.SalidaEncs
            .Where(se => se.SucursalID == sucursalID && se.Estado == "E")
            .SelectMany(se => se.SalidaDets)
                .Where(sd => sd.Lote != null)
                .SumAsync(sd => (decimal?)(sd.Lote.Costo * sd.Cantidad), ct) ?? 0m;

        if (total > 5000)
            {return Result<bool>.Failure($" No se puede ingresar la orden de Salida dado que se ha acumulado y la Sucursal no las ha recibido. ({total}) ");}

        else if ((total + sumaDetalle) > 5000)
            {return Result<bool>.Failure($" No se puede ingresar la orden de Salida actual por sobrepasar mas de lo permitido. ( {total + sumaDetalle} ) ");}

        return Result<bool>.Success(true);
    }
}