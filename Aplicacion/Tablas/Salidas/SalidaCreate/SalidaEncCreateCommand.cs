using System.Net;
using Aplicacion.Core;
using Aplicacion.Tablas.Salidas.SalidasResponse;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Modelo.Entidades;
using Persistencia;
using Seguridad.Interfaces;

namespace Aplicacion.Tablas.Salidas.SalidaCreate;

public class SalidaEncCreateCommand
{
    public record SalidaEncCreateCommandRequest(SalidaEncCreateRequest salidaEncCreateRequest) : IRequest<Result<int>>;

    internal class SalidaEncCreateCommandHandler : IRequestHandler<SalidaEncCreateCommandRequest, Result<int>>
    {
        private readonly BackendContext _backendContext;
        private readonly UserManager<Usuario> _userManager;
        private readonly IUserAccessor _user;
        private readonly IMapper _mapper;
        public SalidaEncCreateCommandHandler(IMapper mapper,BackendContext backendContext, UserManager<Usuario> userManager, IUserAccessor user)
        {
            _backendContext = backendContext;
            _userManager = userManager;
            _user = user;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(SalidaEncCreateCommandRequest request, CancellationToken cancellationToken)
        {
            var usuarioID = _user.GetUserId();
            if (string.IsNullOrEmpty(usuarioID))
            {
                return Result<int>.Failure("No se encontró el UsuarioID en la Autorizacion.", HttpStatusCode.Unauthorized);
            }

            var salidaEnc = new SalidaEnc
            {
                Fecha = DateTime.Now,
                SucursalID = request.salidaEncCreateRequest.SucursalID,
                UsuarioID = usuarioID
            };

            if (request.salidaEncCreateRequest.SucursalID > 0)
            {
                var sucursal = await _backendContext.Sucursales!
                .FirstOrDefaultAsync(x => x.SucursalID == request.salidaEncCreateRequest.SucursalID);

                if (sucursal is null)
                {
                    return Result<int>.Failure("No se encontro la Sucursal.", HttpStatusCode.NotFound);
                }

                salidaEnc.Sucursales = sucursal;
            }

            if (usuarioID is not null)
            {
                var appUsuario = await _userManager.Users!
                .FirstOrDefaultAsync(x => x.Id == usuarioID);
                if (appUsuario is null)
                {
                    return Result<int>.Failure("No se encontro el Usuario.", HttpStatusCode.NotFound);
                }
            }

            var loteListaID = request.salidaEncCreateRequest.SalidasDetalle.Select(linea => linea.LoteID).ToList();
            var lotesDetalle = await _backendContext.Lotes!.Where(lote => loteListaID.Contains(lote.LoteID)).ToListAsync();
            var productoCantidadAgrupado = request.salidaEncCreateRequest.SalidasDetalle
                        .Join(lotesDetalle, detalle => detalle.LoteID, lote => lote.LoteID, (detalle, lote) =>
                        new { ProductoID = lote.ProductoID, Cantidad = detalle.Cantidad })
                        .GroupBy(linea => linea.ProductoID)
                        .Select(grupo => new
                        {
                            ProductoID = grupo.Key,
                            TotalCantidad = grupo.Sum(linea => linea.Cantidad)
                        }).ToList();

            var lotesValidos = new List<LoteCantidadListado>();
            foreach (var linea in productoCantidadAgrupado)
            {
                var productosListado = await _backendContext.Lotes!
                .Where(l => l.ProductoID == linea.ProductoID && l.Cantidad > 0)
                .OrderBy(l => l.FechaVencimiento)
                .ProjectTo<LoteCantidadListado>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

                var seleccionados = DistribucionLotes.Distribuir(
                    productosListado,
                    linea.TotalCantidad,
                    l => l.Cantidad,
                    (l, nuevaCantidad) => l.Cantidad = nuevaCantidad
                );

                lotesValidos.AddRange(seleccionados);
            }

            var salidasDetalles = new List<SalidaDet>();
            decimal sumaDetalle = 0;

            foreach (var detalle in request.salidaEncCreateRequest.SalidasDetalle)
            {
                var lote = await _backendContext.Lotes!
                    .FirstOrDefaultAsync(c => c.LoteID == detalle.LoteID);
                if (lote is null)
                {
                    return Result<int>.Failure($"El Lote con ID {detalle.LoteID} no es válido.", HttpStatusCode.BadRequest);
                }

                if (!lotesValidos.Any(lv => lv.LoteID == detalle.LoteID))
                {
                    return Result<int>.Failure($"El Lote con ID {detalle.LoteID} no es válido se tiene que optar a uno que este con fecha de vencimiento mas proxima.", HttpStatusCode.BadRequest);
                }

                var cantidadPermitida = lotesValidos.FirstOrDefault(lv => lv.LoteID == detalle.LoteID)?.Cantidad;

                if (cantidadPermitida!=detalle.Cantidad)
                {
                    return Result<int>.Failure($"El Lote con ID {detalle.LoteID} tiene una cantidad erronea({detalle.Cantidad}), favor verificar, cantidad permitida: {cantidadPermitida}.", HttpStatusCode.BadRequest);
                }

                sumaDetalle += lote.Costo * detalle.Cantidad;

                if (detalle.Cantidad > lote.Cantidad)
                {
                    return Result<int>.Failure($"No hay suficientes productos en el lote ID {detalle.LoteID}. Disponible: {lote.Cantidad}, solicitado: {detalle.Cantidad}", HttpStatusCode.BadRequest);
                }

                lote.Cantidad -= detalle.Cantidad;

                salidasDetalles.Add(new SalidaDet
                {
                    LoteID = detalle.LoteID,
                    Cantidad = detalle.Cantidad,
                    Lote = lote,
                    Costo = lote.Costo,
                    Salida = salidaEnc
                });
            }

            salidaEnc.SalidaDets = salidasDetalles;

            var total = await _backendContext.SalidaEncs
                            .Where(se => se.SucursalID == salidaEnc.SucursalID && se.Estado == "E")
                            .SelectMany(se => se.SalidaDets)
                                .Where(sd => sd.Lote != null)
                                .SumAsync(sd => (decimal?)(sd.Lote.Costo * sd.Cantidad)) ?? 0m;

            if (total > 5000)
            {
                return Result<int>.Failure($" No se puede ingresar la orden de Salida dado que se ha acumulado y la Sucursal no las ha recibido. ({total}) ", HttpStatusCode.BadRequest);
            }
            else if ((total + sumaDetalle) > 5000)
            {
                return Result<int>.Failure($" No se puede ingresar la orden de Salida actual por sobrepasar mas de lo permitido. ( {total + sumaDetalle} ) ", HttpStatusCode.BadRequest);
            }

            _backendContext.Add(salidaEnc);
            var resultado = await _backendContext.SaveChangesAsync(cancellationToken) > 0;
            return resultado
                        ? Result<int>.Success(salidaEnc.SalidaID)
                        : Result<int>.Failure("No se pudo insertar el registro de la Orden ni su Detalle.", HttpStatusCode.BadRequest);
        }
    }


    public class SalidaEncCreateCommandRequesttValidator : AbstractValidator<SalidaEncCreateCommandRequest>
    {
        public SalidaEncCreateCommandRequesttValidator()
        {
            RuleFor(x => x.salidaEncCreateRequest).SetValidator(new SalidaCreateValidator());
        }
    }
}