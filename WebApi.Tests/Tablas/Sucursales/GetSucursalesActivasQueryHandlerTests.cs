using Aplicacion.Core;
using Aplicacion.Interface;
using Aplicacion.Tablas.Sucursales.DTOSucursales;
using Aplicacion.Tablas.Sucursales.GetSucursalesActivas;
using AutoFixture;
using Moq;

namespace WebApi.Tests.Tablas.Sucursales;

public class GetSucursalesActivasQueryHandlerTests
{
    private Mock<ISucursalService> _sucursalServiceMock;
    private GetSucursalesActivas.GetSucursalesActivasQueryHandler _handler;
    private Fixture _fixture;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _sucursalServiceMock = new Mock<ISucursalService>();
        _handler = new GetSucursalesActivas.GetSucursalesActivasQueryHandler(_sucursalServiceMock.Object);
    }
    [Test]
    public async Task Handle_DevuelveListaDeSucursalesActivas_CuandoExisten()
    {
        // Arrange
        var lista = _fixture.CreateMany<SucursalResponse>(3).ToList();

        _sucursalServiceMock
            .Setup(s => s.ObtenerSucursalesActivas(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<List<SucursalResponse>>.Success(lista));

        var request = new GetSucursalesActivas.GetSucursalesActivasQueryRequest();

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Has.Count.EqualTo(3));
    }
    [Test]
    public async Task Handle_DevuelveListaVacia_SiNoHaySucursales()
    {
        // Arrange
        _sucursalServiceMock
            .Setup(s => s.ObtenerSucursalesActivas(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<List<SucursalResponse>>.Success(new List<SucursalResponse>()));

        var request = new GetSucursalesActivas.GetSucursalesActivasQueryRequest();

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Empty);
    }
    [Test]
    public async Task Handle_DevuelveFallo_SiServicioFalla()
    {
        // Arrange
        _sucursalServiceMock
            .Setup(s => s.ObtenerSucursalesActivas(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<List<SucursalResponse>>.Failure("Error al obtener datos"));

        var request = new GetSucursalesActivas.GetSucursalesActivasQueryRequest();

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("Error al obtener datos"));
    }

}
