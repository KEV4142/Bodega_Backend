namespace Aplicacion.Core;

public static class DistribucionLotes
{
    public static List<T> Distribuir<T>(List<T> lotesOrdenados, int cantidadSolicitada, Func<T, int> getCantidad, Action<T, int> setCantidad)
    {
        var resultado = new List<T>();
        int pedido = cantidadSolicitada;

        foreach (var lote in lotesOrdenados)
        {
            if (pedido == 0) break;

            int disponible = getCantidad(lote);

            if (pedido >= disponible)
            {
                resultado.Add(lote);
                pedido -= disponible;
            }
            else
            {
                var copia = CopiarConNuevaCantidad(lote, pedido, setCantidad);
                resultado.Add(copia);
                pedido = 0;
            }
        }

        return resultado;
    }

    private static T CopiarConNuevaCantidad<T>(T original, int nuevaCantidad, Action<T, int> setCantidad)
    {
        setCantidad(original, nuevaCantidad);
        return original;
    }
}
