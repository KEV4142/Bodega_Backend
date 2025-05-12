using Microsoft.AspNetCore.Identity;

namespace Modelo.Entidades;
public class Usuario : IdentityUser
{
    public string NombreCompleto {get;set;} = null!;
    public virtual ICollection<SalidaEnc> SalidasEnviadas { get; set; } = new List<SalidaEnc>();

    public virtual ICollection<SalidaEnc> SalidasRecibidas { get; set; } = new List<SalidaEnc>();

}