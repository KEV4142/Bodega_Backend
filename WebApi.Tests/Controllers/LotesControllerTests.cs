using System.Net;
using Aplicacion.Core;
using Aplicacion.Tablas.Lotes.DTOLotes;
using Aplicacion.Tablas.Lotes.GetLotesSalida;
using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers;
using WebApi.Tests.Helper;
using static Aplicacion.Tablas.Lotes.GetLotesSalida.GetLotesSalidaQuery;

namespace WebApi.Tests.Controllers;

public class LotesControllerTests
{
    private Mock<ISender> _mockSender;
    private LotesController _controller;
    private Fixture _fixture;

    [SetUp]
    public void Setup()
    {
        _mockSender = new Mock<ISender>();
        _controller = new LotesController(_mockSender.Object);
        _fixture = new Fixture();
        _fixture.Customizations.Add(new ValidDateOnlyBuilder());
    }

    [Test]
    public async Task Disponible_DevuelveOk_SiExitoso()
    {
        // Arrange
        var request = new GetLotesSalidaRequest { ProductoID = 1, Cantidad = 10 };
        var lotes = _fixture.Create<List<LoteCompletoResponse>>();
        var resultado = Result<List<LoteCompletoResponse>>.Success(lotes);

        _mockSender
            .Setup(x => x.Send(It.IsAny<GetLotesSalidaQueryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultado);

        // Act
        var response = await _controller.Disponible(request, CancellationToken.None);

        // Assert
        Assert.That(response.Result, Is.InstanceOf<OkObjectResult>());
        var ok = response.Result as OkObjectResult;
        Assert.That(ok?.Value, Is.EqualTo(lotes));
    }
    [Test]
    public async Task Disponible_DevuelveError_SiFalla()
    {
        // Arrange
        var request = new GetLotesSalidaRequest { ProductoID = 1, Cantidad = 10 };
        var resultado = Result<List<LoteCompletoResponse>>.Failure("Error interno", HttpStatusCode.InternalServerError);

        _mockSender
            .Setup(x => x.Send(It.IsAny<GetLotesSalidaQueryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultado);

        // Act
        var response = await _controller.Disponible(request, CancellationToken.None);

        // Assert
        var statusResult = response.Result as ObjectResult;
        Assert.That(statusResult, Is.Not.Null);
        Assert.That(statusResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
    }

}
