using System.Net;
using Aplicacion.Core;
using Aplicacion.Tablas.Sucursales.DTOSucursales;
using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers;
using static Aplicacion.Tablas.Sucursales.GetSucursalesActivas.GetSucursalesActivas;

namespace WebApi.Tests.Controllers;

public class SucursalesControllerTests
{
    private Mock<ISender> _mockSender;
    private SucursalesController _controller;
    private Fixture _fixture;

    [SetUp]
    public void Setup()
    {
        _mockSender = new Mock<ISender>();
        _controller = new SucursalesController(_mockSender.Object);
        _fixture = new Fixture();
    }

    [Test]
    public async Task GetSucursalesActivos_DevuelveOk_SiExitoso()
    {
        // Arrange
        var sucursales = _fixture.Create<List<SucursalResponse>>();
        var resultado = Result<List<SucursalResponse>>.Success(sucursales);

        _mockSender
            .Setup(x => x.Send(It.IsAny<GetSucursalesActivasQueryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultado);

        // Act
        var response = await _controller.GetSucursalesActivos(CancellationToken.None);

        // Assert
        Assert.That(response.Result, Is.InstanceOf<OkObjectResult>());
        var ok = response.Result as OkObjectResult;
        Assert.That(ok?.Value, Is.EqualTo(sucursales));
    }

    [Test]
    public async Task GetSucursalesActivos_DevuelveError_SiFalla()
    {
        // Arrange
        var resultado = Result<List<SucursalResponse>>.Failure("Error interno", HttpStatusCode.InternalServerError);

        _mockSender
            .Setup(x => x.Send(It.IsAny<GetSucursalesActivasQueryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultado);

        // Act
        var response = await _controller.GetSucursalesActivos(CancellationToken.None);

        // Assert
        var statusResult = response.Result as ObjectResult;
        Assert.That(statusResult, Is.Not.Null);
        Assert.That(statusResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
    }
}
