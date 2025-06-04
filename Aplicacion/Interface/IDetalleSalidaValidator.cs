using Aplicacion.Core;
using Aplicacion.Tablas.Salidas.SalidaCreate;
using Aplicacion.Tablas.Salidas.SalidasResponse;
using Modelo.Entidades;

namespace Aplicacion.Interface;

public interface IDetalleSalidaValidator
{
    Result<Lote> ValidarDetalle(SalidaDetRequest detalle, List<LoteCantidadListado> lotesValidos, List<Lote> lotesDetalle);
}