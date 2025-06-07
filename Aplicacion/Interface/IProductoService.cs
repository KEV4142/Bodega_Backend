using Aplicacion.Core;
using Modelo.Entidades;

namespace Aplicacion.Interface;

public interface IProductoService
{
    Task<Result<Producto>> ObtenerProductoPorIDAsync(int productoID);
    Task<int> TieneInventarioDisponible(int productoID, int cantidad);
}
