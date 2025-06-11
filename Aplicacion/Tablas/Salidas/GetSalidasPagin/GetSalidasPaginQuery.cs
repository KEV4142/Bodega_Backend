using System.Linq.Expressions;
using Aplicacion.Core;
using Aplicacion.Interface;
using Aplicacion.Tablas.Salidas.SalidasResponse;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Modelo.Entidades;

namespace Aplicacion.Tablas.Salidas.GetSalidasPagin;

public class GetSalidasPaginQuery
{
    public record GetSalidasPaginQueryRequest : IRequest<Result<PagedList<SalidaListaResponse>>>
    {
        public GetSalidasPaginRequest? SalidasPaginRequest { get; set; }
    }

    internal class GetSalidasPaginQueryHandler
    : IRequestHandler<GetSalidasPaginQueryRequest, Result<PagedList<SalidaListaResponse>>>
    {
        private readonly ISalidaService _salidaService;
        private readonly IMapper _mapper;

        public GetSalidasPaginQueryHandler(ISalidaService salidaService, IMapper mapper)
        {
            _salidaService = salidaService;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<SalidaListaResponse>>> Handle(
            GetSalidasPaginQueryRequest request,
            CancellationToken cancellationToken
        )
        {
            IQueryable<SalidaEnc> queryable = _salidaService.GetQueryable();
            var predicate = ExpressionBuilder.New<SalidaEnc>();

            if (!string.IsNullOrEmpty(request.SalidasPaginRequest!.SucursalID.ToString()) && request.SalidasPaginRequest.SucursalID != 0)
            {
                predicate = predicate
                .And(y => y.SucursalID == request.SalidasPaginRequest.SucursalID);
            }
            if (request.SalidasPaginRequest.FechaInicio != default && request.SalidasPaginRequest.FechaFinal != default)
            {
                predicate = predicate.And(y => y.Fecha >= request.SalidasPaginRequest.FechaInicio && y.Fecha <= request.SalidasPaginRequest.FechaFinal.AddDays(1).AddTicks(-1));
            }

            queryable = queryable.Where(predicate);
            var salidasQuery = queryable
            .ProjectTo<SalidaListaResponse>(_mapper.ConfigurationProvider)
            .AsQueryable();
            
            if (!string.IsNullOrEmpty(request.SalidasPaginRequest!.OrderBy))
            {
                Expression<Func<SalidaListaResponse, object>>? orderBySelector =
                                request.SalidasPaginRequest.OrderBy!.ToUpper() switch
                                {
                                "CANTIDAD" => salida => salida.Cantidad!,
                                "TOTAL" => salida => salida.Total!,
                                "FECHARECIBIDO" => salida => salida.FechaRecibido!,
                                    _ => salida => salida.SalidaID!
                                };
                bool orderBy = request.SalidasPaginRequest.OrderAsc ?? true;

                salidasQuery = orderBy
                            ? salidasQuery.OrderBy(orderBySelector)
                            : salidasQuery.OrderByDescending(orderBySelector);
            }

            var pagination = await PagedList<SalidaListaResponse>.CreateAsync(
                salidasQuery,
                request.SalidasPaginRequest.PageNumber,
                request.SalidasPaginRequest.PageSize
            );

            return Result<PagedList<SalidaListaResponse>>.Success(pagination);
        }


    }

}