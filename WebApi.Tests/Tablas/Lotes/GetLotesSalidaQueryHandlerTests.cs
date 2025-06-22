using System.Net;
using Aplicacion.Core;
using Aplicacion.Interface;
using Aplicacion.Tablas.Lotes.DTOLotes;
using Aplicacion.Tablas.Lotes.GetLotesSalida;
using AutoFixture;
using Modelo.Entidades;
using Moq;
using WebApi.Tests.Helper;

namespace WebApi.Tests.Tablas.Lotes;

public class GetLotesSalidaQueryHandlerTests
{
    private Mock<IDistribuidorLotes> _distribuidorMock;
    private Mock<IProductoService> _productoServiceMock;
    private Mock<ILoteService> _loteServiceMock;
    private GetLotesSalidaQuery.GetLotesSalidaQueryHandler _handler;
    private IFixture _fixture;
    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _fixture.Customizations.Add(new ValidDateOnlyBuilder());
        _fixture.Customize<Lote>(c => c
            .Without(x => x.Costo)
            .Do(x => x.Costo = 50.0m));
        _fixture.Customize<SalidaDet>(c => c
            .Without(x => x.Costo)
            .Do(x => x.Costo = 50.0m));
        _distribuidorMock = new Mock<IDistribuidorLotes>();
        _productoServiceMock = new Mock<IProductoService>();
        _loteServiceMock = new Mock<ILoteService>();

        _handler = new GetLotesSalidaQuery.GetLotesSalidaQueryHandler(
            _distribuidorMock.Object,
            _productoServiceMock.Object,
            _loteServiceMock.Object
        );
    }

    [Test]
    public async Task Handle_ReturnsLotesDistribuidos_CuandoDataEsValido()
    {
        // Arrange
        var request = new GetLotesSalidaQuery.GetLotesSalidaQueryRequest(
            new GetLotesSalidaRequest { ProductoID = 1, Cantidad = 10 }
        );

        var producto = _fixture.Create<Producto>();
        var lotesDisponibles = _fixture.CreateMany<LoteCompletoResponse>(5).ToList();
        var lotesDistribuidos = _fixture.CreateMany<LoteCompletoResponse>(2).ToList();

        _productoServiceMock
            .Setup(x => x.ObtenerProductoPorID(request.getLotesSalidaRequest.ProductoID, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Producto>.Success(producto));

        _productoServiceMock
            .Setup(x => x.TieneInventarioDisponible(request.getLotesSalidaRequest.ProductoID, It.IsAny<CancellationToken>()))
            .ReturnsAsync(20);

        _loteServiceMock
            .Setup(x => x.ObtenerLotesDisponiblesOrdenados(request.getLotesSalidaRequest.ProductoID, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lotesDisponibles);

        _distribuidorMock
            .Setup(x => x.Distribuir(lotesDisponibles, 10, It.IsAny<Func<LoteCompletoResponse, int>>(), It.IsAny<Action<LoteCompletoResponse, int>>()))
            .Returns(lotesDistribuidos);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(lotesDistribuidos));
    }
    [Test]
    public async Task Handle_ReturnsFailure_WhenProductoNoExiste()
    {
        // Arrange
        var request = new GetLotesSalidaQuery.GetLotesSalidaQueryRequest(
            new GetLotesSalidaRequest { ProductoID = 99, Cantidad = 5 }
        );

        _productoServiceMock
            .Setup(x => x.ObtenerProductoPorID(request.getLotesSalidaRequest.ProductoID, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Producto>.Failure("Producto no encontrado", HttpStatusCode.NotFound));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("Producto no encontrado"));
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
    [Test]
    public async Task Handle_ReturnsFailure_WhenInventarioEsInsuficiente()
    {
        // Arrange
        var request = new GetLotesSalidaQuery.GetLotesSalidaQueryRequest(
            new GetLotesSalidaRequest { ProductoID = 1, Cantidad = 15 }
        );

        var producto = _fixture.Create<Producto>();

        _productoServiceMock
            .Setup(x => x.ObtenerProductoPorID(request.getLotesSalidaRequest.ProductoID, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Producto>.Success(producto));

        _productoServiceMock
            .Setup(x => x.TieneInventarioDisponible(request.getLotesSalidaRequest.ProductoID, It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Does.Contain("No se tiene suficiente Inventario"));
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
}
