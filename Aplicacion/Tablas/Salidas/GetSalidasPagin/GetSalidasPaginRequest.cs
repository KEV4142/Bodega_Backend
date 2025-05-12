using Aplicacion.Core;

namespace Aplicacion.Tablas.Salidas.GetSalidasPagin;
public class GetSalidasPaginRequest : PagingParams
{
    public int SucursalID { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFinal { get; set; }
}