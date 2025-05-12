namespace Aplicacion.Tablas.Sucursales.DTOSucursales;
public record SucursalResponse(
    int SucursalID,
    string Descripcion,
    string Direccion,
    string Estado
);