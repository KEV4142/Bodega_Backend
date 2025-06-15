using System.ComponentModel.DataAnnotations;

namespace Modelo.Entidades;

public partial class Lote
{
    [Key]
    public int LoteID { get; set; }
    [Required]
    public int ProductoID { get; set; }
    [Required]
    public DateOnly FechaVencimiento { get; set; }
    [Required]
    [Range(typeof(decimal), "0.01", "1000000.00", ErrorMessage = "El Costo debe ser mayor que 0.")]
    public decimal Costo { get; set; }
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor o igual a 0.")]
    public int Cantidad { get; set; }
    public byte[] CampoConcurrencia { get; set; } = null!;

    public virtual Producto Producto { get; set; } = null!;

    public virtual ICollection<SalidaDet> SalidaDets { get; set; } = new List<SalidaDet>();
}
