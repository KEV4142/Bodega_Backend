using Aplicacion.Core;
using Aplicacion.Interface;
using Aplicacion.Tablas.Salidas.SalidaCreate;
using Aplicacion.Tablas.Salidas.SalidasResponse;
using Modelo.Entidades;

namespace Aplicacion.Service;

public class SalidaDetListBuilder : ISalidaDetListBuilder
{
    private readonly IDetalleSalidaValidator _detalleSalidaValidator;

    public SalidaDetListBuilder(IDetalleSalidaValidator validador)
    {
        _detalleSalidaValidator = validador;
    }
    public Result<SalidaDetConstruccionResultado> Construir(
        List<SalidaDetRequest> detallesRequest,
        List<Lote> lotesDetalle,
        List<LoteCantidadListado> lotesValidos,
        SalidaEnc salidaEnc
    )
    {
        var salidasDetalles = new List<SalidaDet>();
        decimal sumaDetalle = 0;

        foreach (var detalle in detallesRequest)
        {
            var resultadoValidacion = _detalleSalidaValidator.ValidarDetalle(detalle, lotesValidos, lotesDetalle);
            if (!resultadoValidacion.IsSuccess)
                return Result<SalidaDetConstruccionResultado>.Failure(resultadoValidacion.Error!, resultadoValidacion.StatusCode);

            var lote = resultadoValidacion.Value!;
            sumaDetalle += lote.Costo * detalle.Cantidad;
            lote.Cantidad -= detalle.Cantidad;

            salidasDetalles.Add(new SalidaDet
            {
                LoteID = detalle.LoteID,
                Cantidad = detalle.Cantidad,
                Lote = lote,
                Costo = lote.Costo,
                Salida = salidaEnc
            });
        }

        var resultadoFinal = new SalidaDetConstruccionResultado
        {
            SalidasDetalles = salidasDetalles,
            Total = sumaDetalle
        };

        return Result<SalidaDetConstruccionResultado>.Success(resultadoFinal);
    }
}
