using System.ComponentModel.DataAnnotations;

namespace Modelo.Entidades;

public partial class Sucursal
{
    [Key]
    public int SucursalID { get; set; }
    [Required]
    [StringLength(100)]
    public string Descripcion { get; set; } = null!;
    [Required]
    [StringLength(100)]
    public string Direccion { get; set; } = null!;
    [Required]
    [StringLength(1)]
    [RegularExpression("^[AIB]$", ErrorMessage = "El estado solo puede ser 'A' o 'I' o 'B'.")]
    public string Estado { get; set; } = null!;

    public virtual ICollection<SalidaEnc> SalidaEncs { get; set; } = new List<SalidaEnc>();
}
