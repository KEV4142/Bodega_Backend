namespace Modelo.Entidades;

public partial class Lote
{
    public int LoteID { get; set; }

    public int ProductoID { get; set; }

    public DateOnly FechaVencimiento { get; set; }

    public decimal Costo { get; set; }

    public int Cantidad { get; set; }

    public virtual Producto Producto { get; set; } = null!;

    public virtual ICollection<SalidaDet> SalidaDets { get; set; } = new List<SalidaDet>();
}
