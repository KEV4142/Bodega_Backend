namespace Modelo.Entidades;

public partial class SalidaEnc
{
    public int SalidaID { get; set; }

    public int SucursalID { get; set; }

    public DateTime Fecha { get; set; }

    public DateTime? FechaRecibido { get; set; }

    public string UsuarioID { get; set; } = null!;

    public string? UsuarioRecibe { get; set; }

    public string Estado { get; set; } = null!;

    public virtual ICollection<SalidaDet> SalidaDets { get; set; } = new List<SalidaDet>();

    public virtual Sucursal Sucursales { get; set; } = null!;
    public virtual Usuario Usuario { get; set; } = null!;

    public virtual Usuario? UsuarioRecibeRelacion { get; set; }
}
