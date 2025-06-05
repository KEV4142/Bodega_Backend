namespace Aplicacion.Interface;

public interface IDistribuidorLotes
{
    List<T> Distribuir<T>(List<T> lotesOrdenados, int cantidadSolicitada, Func<T, int> getCantidad, Action<T, int> setCantidad);
}
