using FluentValidation;

namespace Aplicacion.Tablas.Salidas.SalidaUpdateEstado;
public class SalidaUpdateEstadoValidator : AbstractValidator<SalidaUpdateEstadoRequest>
{
    public SalidaUpdateEstadoValidator()
    {
        RuleFor(x => x.Estado)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Campo Estado es Obligatorio.")
            .NotEmpty().WithMessage("El campo Estado se encuentra en blanco.")
            .Must(estado => estado == "r" || estado == "R" ).WithMessage("El Estado debe ser R.");
    }
}