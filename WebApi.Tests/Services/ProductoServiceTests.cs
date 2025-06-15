using Aplicacion.Service;
using Aplicacion.Tablas.Productos.DTOProductos;
using AutoFixture;
using AutoMapper;
using Modelo.Entidades;
using Modelo.Interfaces;
using Moq;
using System.Net;
using WebApi.Tests.Helper;

namespace WebApi.Tests.Services;

public class ProductoServiceTests
{
    private Mock<IProductoRepository> _mockRepo;
    private Mock<IMapper> _mockMapper;
    private ProductoService _service;
    private Fixture _fixture;

    [SetUp]
    public void Setup()
    {
        _mockRepo = new Mock<IProductoRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new ProductoService(_mockRepo.Object, _mockMapper.Object);
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
    }

    [Test]
    public async Task ObtenerProductoPorIDResponse_DevuelveSuccess_SiExiste()
    {
        // Arrange
        var producto = new Producto
        {
            ProductoID = 1,
            Descripcion = "Prueba",
            Lotes = new List<Lote>
            {
                new Lote
                {
                    LoteID = 1,
                    FechaVencimiento = new DateOnly(2025, 12, 31)
                }
            }
        };

        var productoResponse = _fixture.Create<ProductoResponse>();

        _mockRepo
            .Setup(r => r.ObtenerPorIDAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(producto);

        _mockMapper
            .Setup(m => m.Map<ProductoResponse>(producto))
            .Returns(productoResponse);

        // Act
        var result = await _service.ObtenerProductoPorIDResponse(1, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(productoResponse));
    }

    [Test]
    public async Task ObtenerProductoPorIDResponse_DevuelveFailure_SiNoExiste()
    {
        // Arrange
        _mockRepo
            .Setup(r => r.ObtenerPorIDAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Producto?)null);

        // Act
        var result = await _service.ObtenerProductoPorIDResponse(999, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        Assert.That(result.Error, Is.EqualTo("No se encontró el Producto."));
    }

    [Test]
    public async Task TieneInventarioDisponible_DevuelveCantidad()
    {
        _mockRepo
            .Setup(r => r.ObtenerInventarioDisponibleAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(15);

        var disponible = await _service.TieneInventarioDisponible(1, CancellationToken.None);

        Assert.That(disponible, Is.EqualTo(15));
    }
    [Test]
    public async Task ObtenerProductoPorID_DevuelveSuccess_SiExiste()
    {
        // Arrange
        var producto = _fixture.Build<Producto>()
            .With(p => p.ProductoID, 1)
            .With(p => p.Descripcion, "Producto Test")
            .With(p => p.Lotes, new List<Lote>{
                                                new Lote
                                                {
                                                    LoteID = 1,
                                                    FechaVencimiento = new DateOnly(2025, 12, 31)
                                                }
                                            })
            .Create();

        _mockRepo
            .Setup(r => r.ObtenerPorIDAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(producto);

        // Act
        var result = await _service.ObtenerProductoPorID(1, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(producto));
    }

    [Test]
    public async Task ObtenerProductoPorID_DevuelveFailure_SiNoExiste()
    {
        // Arrange
        _mockRepo
            .Setup(r => r.ObtenerPorIDAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Producto?)null);

        // Act
        var result = await _service.ObtenerProductoPorID(999, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("No se encontró el Producto."));
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
    [Test]
    public async Task ObtenerProductosActivos_DevuelveSuccess_SiHayProductos()
    {
        // Arrange
        var productos = _fixture.CreateMany<Producto>(3).ToList();
        var productosDTO = _fixture.CreateMany<ProductoResponse>(3).ToList();

        _mockRepo
            .Setup(r => r.ObtenerProductosActivosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(productos);

        _mockMapper
            .Setup(m => m.Map<List<ProductoResponse>>(productos))
            .Returns(productosDTO);

        // Act
        var result = await _service.ObtenerProductosActivos(CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(productosDTO));
    }

    [Test]
    public async Task ObtenerProductosActivos_DevuelveListaVacia_SiNoHayProductos()
    {
        // Arrange
        var productos = new List<Producto>
        {
            new Producto
            {
                ProductoID = 1,
                Descripcion = "Producto A",
                Lotes = new List<Lote>
                {
                    new Lote { LoteID = 1, FechaVencimiento = DateOnly.FromDateTime(DateTime.Today.AddMonths(6)), Costo = 10.50m }
                }
            }
        };

        var productosDTO = new List<ProductoResponse>();

        _mockRepo
            .Setup(r => r.ObtenerProductosActivosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(productos);

        _mockMapper
            .Setup(m => m.Map<List<ProductoResponse>>(productos))
            .Returns(productosDTO);

        // Act
        var result = await _service.ObtenerProductosActivos(CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Empty);
    }
}
