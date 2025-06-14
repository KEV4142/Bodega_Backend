using System.Net;
using Aplicacion.Core;
using Aplicacion.Interface;
using Modelo.Entidades;
using Modelo.Interfaces;

namespace Aplicacion.Service;

public class SalidaService : ISalidaService
{
    private readonly ISalidaEncRepository _salidaEncRepository;

    public SalidaService(ISalidaEncRepository salidaEncRepository)
    {
        _salidaEncRepository = salidaEncRepository;
    }
    public async Task<Result<SalidaEnc>> ObtenerSalidaPorID(int salidaID, CancellationToken cancellationToken)
    {
        var salida = await _salidaEncRepository.ObtenerPorIDAsync(salidaID, cancellationToken);

        if (salida is null)
            return Result<SalidaEnc>.Failure("La Orden de Salida no existe.", HttpStatusCode.NotFound);

        return Result<SalidaEnc>.Success(salida);
    }

    public async Task<Result<int>> CambiarEstadoSalida(SalidaEnc salida, string nuevoEstado, string usuarioID, CancellationToken cancellationToken)
    {
        if (salida.Estado.Equals("R", StringComparison.OrdinalIgnoreCase))
            return Result<int>.Failure("La Orden de Salida ya ha sido recibida.", HttpStatusCode.BadRequest);
        if (salida.Fecha >= salida.FechaRecibido)
            return Result<int>.Failure("La fecha de recibido debe ser mayor que la fecha de emisión.", HttpStatusCode.BadRequest);
            
        var resultado = await _salidaEncRepository.ActualizarEstadoAsync(salida, nuevoEstado, usuarioID, cancellationToken);

        if (!resultado)
            return Result<int>.Failure("Errores en la actualización del estado de la Orden Salida.", HttpStatusCode.BadRequest);

        return Result<int>.Success(salida.SalidaID);
    }
    public async Task<Result<int>> RegistrarSalida(SalidaEnc salidaEnc, CancellationToken cancellationToken)
    {
        var resultado = await _salidaEncRepository.InsertarAsync(salidaEnc, cancellationToken);

        if (!resultado)
            return Result<int>.Failure("No se pudo insertar el registro de la Orden ni su Detalle.", HttpStatusCode.BadRequest);

        return Result<int>.Success(salidaEnc.SalidaID);
    }

    public IQueryable<SalidaEnc> GetQueryable()
    {
        return _salidaEncRepository.GetQueryable();
    }

    public async Task<decimal> ObtenerTotalCostoPendientePorSucursal(int sucursalID, CancellationToken cancellationToken)
    {
        return await _salidaEncRepository.ObtenerTotalCostoPendientePorSucursalAsync(sucursalID,cancellationToken);
    }
}
