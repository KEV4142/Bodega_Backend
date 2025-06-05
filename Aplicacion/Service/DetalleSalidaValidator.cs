using Aplicacion.Core;
using Aplicacion.Interface;
using Aplicacion.Tablas.Salidas.SalidaCreate;
using Aplicacion.Tablas.Salidas.SalidasResponse;
using Modelo.Entidades;

namespace Aplicacion.Service;

public class DetalleSalidaValidator : IDetalleSalidaValidator
{
    public Result<Lote> ValidarDetalle(SalidaDetRequest detalle, List<LoteCantidadListado> lotesValidos, List<Lote> lotesDetalle)
    {
        var lote = lotesDetalle.FirstOrDefault(linea => linea.LoteID == detalle.LoteID && linea.Cantidad>0);
        if (lote is null)
            {return Result<Lote>.Failure($"El Lote con ID {detalle.LoteID} no es válido. Favor revisar ID o Cantidad.");}
            
        if (!lotesValidos.Any(lv => lv.LoteID == detalle.LoteID))
            {return Result<Lote>.Failure($"El Lote con ID {detalle.LoteID} no es válido, se tiene que optar a uno que este con fecha de vencimiento mas proxima.");}

        var cantidadPermitida = lotesValidos.FirstOrDefault(lv => lv.LoteID == detalle.LoteID)?.Cantidad;
        if (cantidadPermitida != detalle.Cantidad)
            {return Result<Lote>.Failure($"El Lote con ID {detalle.LoteID} tiene una cantidad erronea({detalle.Cantidad}), favor verificar, cantidad permitida: {cantidadPermitida}.");}

        if (detalle.Cantidad > lote.Cantidad)
            {return Result<Lote>.Failure($"No hay suficientes productos en el lote ID {detalle.LoteID}. Disponible: {lote.Cantidad}, solicitado: {detalle.Cantidad}");}

        return Result<Lote>.Success(lote);
    }
}