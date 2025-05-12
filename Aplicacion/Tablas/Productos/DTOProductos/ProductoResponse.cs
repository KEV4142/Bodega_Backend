namespace Aplicacion.Tablas.Productos.DTOProductos;

public record ProductoResponse(
    int ProductoID,
    string Descripcion,
    string Estado
);