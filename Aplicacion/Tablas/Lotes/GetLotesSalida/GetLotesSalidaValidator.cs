using FluentValidation;

namespace Aplicacion.Tablas.Lotes.GetLotesSalida;
public class GetLotesSalidaValidator : AbstractValidator<GetLotesSalidaRequest>
{
    public GetLotesSalidaValidator()
    {
        RuleFor(x => x.ProductoID)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Campo Producto es Obligatorio.")
            .NotEmpty().WithMessage("El campo Producto esta en blanco.")
            .GreaterThan(0).WithMessage("El Campo Producto es obligatorio(Mayor a 0).");
        RuleFor(x => x.Cantidad)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Campo Cantidad es Obligatorio.")
            .NotEmpty().WithMessage("El campo Cantidad esta en blanco.")
            .GreaterThan(0).WithMessage("El Campo Cantidad es obligatorio(Mayor a 0).");
    }
}