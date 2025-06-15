using Aplicacion.Core;
using Aplicacion.Interface;
using Aplicacion.Tablas.Productos.DTOProductos;
using AutoFixture;
using Moq;
using System.Net;
using static Aplicacion.Tablas.Productos.GetProducto.GetProductoQuery;

namespace WebApi.Tests.Tablas.Productos;

public class GetProductoQueryHandlerTests
{
    private Mock<IProductoService> _mockProductoService;
    private GetProductoQueryHandler _handler;
    private Fixture _fixture;

    [SetUp]
    public void Setup()
    {
        _mockProductoService = new Mock<IProductoService>();
        _handler = new GetProductoQueryHandler(_mockProductoService.Object);
        _fixture = new Fixture();
    }

    [Test]
    public async Task Handle_DevuelveSuccess_SiProductoExiste()
    {
        // Arrange
        var producto = _fixture.Create<ProductoResponse>();
        var expectedResult = Result<ProductoResponse>.Success(producto);

        _mockProductoService
            .Setup(s => s.ObtenerProductoPorIDResponse(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var request = new GetProductoQueryRequest { ProductoID = 1 };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(producto));
    }

    [Test]
    public async Task Handle_DevuelveFailure_SiProductoNoExiste()
    {
        // Arrange
        var errorResult = Result<ProductoResponse>.Failure("No encontrado", HttpStatusCode.NotFound);

        _mockProductoService
            .Setup(s => s.ObtenerProductoPorIDResponse(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(errorResult);

        var request = new GetProductoQueryRequest { ProductoID = 999 };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        Assert.That(result.Error, Is.EqualTo("No encontrado"));
    }
}
