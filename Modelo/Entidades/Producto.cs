using System.ComponentModel.DataAnnotations;

namespace Modelo.Entidades;

public partial class Producto
{
    [Key]
    public int ProductoID { get; set; }
    [Required]
    [StringLength(100)]
    public string Descripcion { get; set; } = null!;
    [Required]
    [StringLength(1)]
    [RegularExpression("^[AIB]$", ErrorMessage = "El estado solo puede ser 'A' o 'I' o 'B'.")]
    public string Estado { get; set; } = null!;

    public virtual ICollection<Lote> Lotes { get; set; } = new List<Lote>();
}
