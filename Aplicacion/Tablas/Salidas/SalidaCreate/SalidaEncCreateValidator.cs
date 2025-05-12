using FluentValidation;

namespace Aplicacion.Tablas.Salidas.SalidaCreate;
public class SalidaCreateValidator : AbstractValidator<SalidaEncCreateRequest>
{
public SalidaCreateValidator()
    {
        RuleFor(x => x.SucursalID).GreaterThan(0).WithMessage("Se debe tener seleccionado una Sucursal.");
        RuleForEach(x => x.SalidasDetalle).SetValidator(new SalidaDetValidator()).WithMessage("Error en los detalles de la Orden Salida.");
        RuleFor(x => x.SalidasDetalle).NotEmpty().WithMessage("La lista de detalles no puede estar vacía.");
    }
}

public class SalidaDetValidator : AbstractValidator<SalidaDetRequest>
{
    public SalidaDetValidator()
    {
        RuleFor(x => x.LoteID).GreaterThan(0).WithMessage("El campo Lote no es valido o debe ser mayor que 0.");
        RuleFor(x => x.Cantidad).GreaterThan(0).WithMessage("El campo Cantidad no es valido o debe ser mayor que 0.");
    }
}