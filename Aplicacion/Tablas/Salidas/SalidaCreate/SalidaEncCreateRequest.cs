namespace Aplicacion.Tablas.Salidas.SalidaCreate;
public class SalidaEncCreateRequest
{
    // public DateTime Fecha { get; set; }

    public int SucursalID { get; set; }

    public List<SalidaDetRequest> SalidasDetalle { get; set; } = new List<SalidaDetRequest>();
    
}
public class SalidaDetRequest
{
    public int LoteID { get; set; }
    public int Cantidad { get; set; }
}