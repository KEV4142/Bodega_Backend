using Aplicacion.Core;
using Aplicacion.Tablas.Salidas.SalidaCreate;
using Aplicacion.Tablas.Salidas.SalidasResponse;
using Modelo.Entidades;

namespace Aplicacion.Interface;

public interface ISalidaDetListBuilder
{
    Result<SalidaDetConstruccionResultado> Construir(
    List<SalidaDetRequest> detallesRequest,
    List<Lote> lotesDetalle,
    List<LoteCantidadListado> lotesValidos,
    SalidaEnc salidaEnc
);
}
