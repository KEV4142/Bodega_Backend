using System.ComponentModel.DataAnnotations;

namespace Modelo.Entidades;

public partial class SalidaDet
{
    public int SalidaID { get; set; }

    public int LoteID { get; set; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que 0.")]
    public int Cantidad { get; set; }
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "El Costo debe ser mayor que 0.")]
    public decimal Costo { get; set; }

    public virtual Lote Lote { get; set; } = null!;

    public virtual SalidaEnc Salida { get; set; } = null!;
}
