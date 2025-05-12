namespace Aplicacion.Tablas.Lotes.DTOLotes;

public class LoteCompletoResponse{
    public  int LoteID { get; set; }
    public  int ProductoID { get; set; }
    public  string Descripcion { get; set; } = null!;
    public  int Cantidad { get; set; }
    public  decimal Costo { get; set; }
    public  DateOnly FechaVencimiento { get; set; }
}
public record LoteResponse(
    int LoteID,
    int ProductoID,
    int Cantidad,
    decimal Costo,
    DateOnly FechaVencimiento
);