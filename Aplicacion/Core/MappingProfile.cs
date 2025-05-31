using Aplicacion.Tablas.Sucursales.DTOSucursales;
using Aplicacion.Tablas.Productos.DTOProductos;
using AutoMapper;
using Modelo.Entidades;
using Aplicacion.Tablas.Lotes.DTOLotes;
using Aplicacion.Tablas.Salidas.SalidasResponse;

namespace Aplicacion.Core;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Sucursal, SucursalResponse>();
        CreateMap<Producto, ProductoResponse>();
        CreateMap<Lote, LoteCompletoResponse>()
            .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Producto.Descripcion));
        CreateMap<Lote, LoteCantidadListado>();
        CreateMap<SalidaEnc, SalidaListaResponse>()
            .ForMember(dest => dest.Cantidad, opt => opt.MapFrom(src => src.SalidaDets.Sum(sd => sd.Cantidad)))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.SalidaDets.Sum(sd => sd.Cantidad * sd.Lote.Costo)))
            .ForMember(dest => dest.UsuarioRecibeNonmbre, opt => opt.MapFrom(src => src.UsuarioRecibeRelacion!.UserName));

    }
}