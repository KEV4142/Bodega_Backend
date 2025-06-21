using System.Net;
using Aplicacion.Core;
using Aplicacion.Tablas.Salidas.GetSalidasPagin;
using Aplicacion.Tablas.Salidas.SalidaCreate;
using Aplicacion.Tablas.Salidas.SalidasResponse;
using Aplicacion.Tablas.Salidas.SalidaUpdateEstado;
using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers;
using static Aplicacion.Tablas.Salidas.GetSalidasPagin.GetSalidasPaginQuery;
using static Aplicacion.Tablas.Salidas.SalidaCreate.SalidaEncCreateCommand;
using static Aplicacion.Tablas.Salidas.SalidaUpdateEstado.SalidaUpdateEstadoCommand;

namespace WebApi.Tests.Controllers;

public class SalidasControllerTests
{
    private Mock<ISender> _senderMock;
    private SalidasController _controller;
    private Fixture _fixture;
    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _senderMock = new Mock<ISender>();
        _controller = new SalidasController(_senderMock.Object);
    }
    [Test]
    public async Task Salida_RetornaOk_SiOperacionExitosa()
    {
        var request = new SalidaEncCreateRequest { SucursalID = 1, SalidasDetalle = new() };
        var command = new SalidaEncCreateCommandRequest(request);
        _senderMock.Setup(s => s.Send(It.IsAny<SalidaEncCreateCommandRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<int>.Success(123));

        var result = await _controller.Salida(request, CancellationToken.None);

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        Assert.That(((OkObjectResult)result.Result!).Value, Is.EqualTo(123));
    }
    [Test]
    public async Task Salida_RetornaBadRequest_SiFallaValidacion()
    {
        var request = new SalidaEncCreateRequest();
        _senderMock.Setup(s => s.Send(It.IsAny<SalidaEncCreateCommandRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<int>.Failure("Error de validación", HttpStatusCode.BadRequest));

        var result = await _controller.Salida(request, CancellationToken.None);

        var statusResult = result.Result as ObjectResult;
        Assert.That(statusResult!.StatusCode, Is.EqualTo(400));
    }
    [Test]
    public async Task Salida_RetornaConflict_SiExisteConflictoConDatos()
    {
        // Arrange
        var request = _fixture.Create<SalidaEncCreateRequest>();
        var resultadoConflict = Result<int>.Failure("Conflicto de datos", HttpStatusCode.Conflict);

        _senderMock
            .Setup(s => s.Send(It.IsAny<SalidaEncCreateCommandRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultadoConflict);

        // Act
        var result = await _controller.Salida(request, CancellationToken.None);

        // Assert
        var statusResult = result.Result as ObjectResult;
        Assert.That(statusResult, Is.Not.Null);
        Assert.That(statusResult!.StatusCode, Is.EqualTo(409));
        Assert.That(statusResult.Value, Is.EqualTo(resultadoConflict));
    }

    [Test]
    public async Task PaginationSalidas_RetornaOk_SiExitoso()
    {
        var request = _fixture.Create<GetSalidasPaginRequest>();
        var salidas = _fixture.CreateMany<SalidaListaResponse>(5).ToList();

        var data = new PagedList<SalidaListaResponse>(salidas, count: 50, pageNumber: request.PageNumber, pageSize: request.PageSize);

        _senderMock.Setup(s => s.Send(It.IsAny<GetSalidasPaginQueryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PagedList<SalidaListaResponse>>.Success(data));

        var result = await _controller.PaginationSalidas(request, CancellationToken.None);

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        Assert.That(((OkObjectResult)result.Result!).Value, Is.EqualTo(data));
    }

    [Test]
    public async Task PaginationSalidas_RetornaUnauthorized_SiFalla()
    {
        var request = _fixture.Create<GetSalidasPaginRequest>();

        _senderMock.Setup(s => s.Send(It.IsAny<GetSalidasPaginQueryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PagedList<SalidaListaResponse>>.Failure("No autorizado", HttpStatusCode.Unauthorized));

        var result = await _controller.PaginationSalidas(request, CancellationToken.None);

        var status = result.Result as ObjectResult;
        Assert.That(status!.StatusCode, Is.EqualTo(401));
    }

    [Test]
    public async Task SalidaUpdateEstado_RetornaOk_SiActualizaEstado()
    {
        var request = _fixture.Create<SalidaUpdateEstadoRequest>();

        _senderMock.Setup(s => s.Send(It.IsAny<SalidaUpdateEstadoCommandRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<int>.Success(1));

        var result = await _controller.SalidaUpdateEstado(request, 1, CancellationToken.None);

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var obj = (OkObjectResult)result.Result!;
        Assert.That(((Result<int>)obj.Value!).Value, Is.EqualTo(1));
    }

    [Test]
    public async Task SalidaUpdateEstado_RetornaNotFound_SiNoExisteSalida()
    {
        var request = _fixture.Create<SalidaUpdateEstadoRequest>();

        _senderMock.Setup(s => s.Send(It.IsAny<SalidaUpdateEstadoCommandRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<int>.Failure("No existe", HttpStatusCode.NotFound));

        var result = await _controller.SalidaUpdateEstado(request, 999, CancellationToken.None);

        var status = result.Result as ObjectResult;
        Assert.That(status!.StatusCode, Is.EqualTo(404));
    }


}
