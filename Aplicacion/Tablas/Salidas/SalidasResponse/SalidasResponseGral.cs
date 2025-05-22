namespace Aplicacion.Tablas.Salidas.SalidasResponse;
public class SalidaListaResponse{
    public int SalidaID { get; set; }
    public DateTime Fecha { get; set; }
    public DateTime? FechaRecibido { get; set; }
    public string? UsuarioRecibeNonmbre { get; set; }
    public string Estado { get; set; } = null!;
    public int Cantidad { get; set; }
    public decimal Total { get; set; }
}

public class LoteCantidadListado{
    public int LoteID { get; set; }
    public int Cantidad { get; set; }
} 