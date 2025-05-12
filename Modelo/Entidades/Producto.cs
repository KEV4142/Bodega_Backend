namespace Modelo.Entidades;

public partial class Producto
{
    public int ProductoID { get; set; }

    public string Descripcion { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public virtual ICollection<Lote> Lotes { get; set; } = new List<Lote>();
}
