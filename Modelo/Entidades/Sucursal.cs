namespace Modelo.Entidades;

public partial class Sucursal
{
    public int SucursalID { get; set; }

    public string Descripcion { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public virtual ICollection<SalidaEnc> SalidaEncs { get; set; } = new List<SalidaEnc>();
}
