using Aplicacion.Service;
using Aplicacion.Tablas.Lotes.DTOLotes;
using Aplicacion.Tablas.Salidas.SalidasResponse;
using AutoFixture;
using AutoMapper;
using Modelo.Entidades;
using Modelo.Interfaces;
using Moq;
using WebApi.Tests.Helper;

namespace WebApi.Tests.Services;

public class LoteServiceTests
{
    private IFixture _fixture;
    private Mock<ILoteRepository> _mockRepo;
    private Mock<IMapper> _mockMapper;
    private LoteService _service;
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
        _mockRepo = _fixture.Freeze<Mock<ILoteRepository>>();
        _mockMapper = _fixture.Freeze<Mock<IMapper>>();
        _service = new LoteService(_mockRepo.Object, _mockMapper.Object);
    }
    [Test]
    public async Task ObtenerLotesDisponiblesOrdenados_DevuelveListaMapeada()
    {
        // Arrange
        var productoID = 1;
        var lotes = _fixture.CreateMany<Lote>(3).ToList();
        var response = _fixture.CreateMany<LoteCompletoResponse>(3).ToList();

        _mockRepo.Setup(r => r.ObtenerLotesDisponiblesOrdenadosAsync(productoID, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lotes);

        _mockMapper.Setup(m => m.Map<List<LoteCompletoResponse>>(lotes))
            .Returns(response);

        // Act
        var result = await _service.ObtenerLotesDisponiblesOrdenados(productoID, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(response));
    }

    [Test]
    public async Task ObtenerLotesDisponiblesParaProducto_DevuelveListaMapeada()
    {
        // Arrange
        var productoID = 1;
        var lotes = _fixture.CreateMany<Lote>(2).ToList();
        var response = _fixture.CreateMany<LoteCantidadListado>(2).ToList();

        _mockRepo.Setup(r => r.ObtenerLotesDisponiblesParaProductoAsync(productoID, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lotes);

        _mockMapper.Setup(m => m.Map<List<LoteCantidadListado>>(lotes))
            .Returns(response);

        // Act
        var result = await _service.ObtenerLotesDisponiblesParaProducto(productoID, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(response));
    }

    [Test]
    public async Task ObtenerLotesPorIDLista_DevuelveListaDirecta()
    {
        // Arrange
        var ids = new List<int> { 1, 2, 3 };
        var lotes = _fixture.CreateMany<Lote>(3).ToList();

        _mockRepo.Setup(r => r.ObtenerLotesPorIDListaAsync(ids, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lotes);

        // Act
        var result = await _service.ObtenerLotesPorIDLista(ids, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(lotes));
    }
    [Test]
    public async Task ObtenerLotesPorIDLista_DevuelveListaVacia_SiNoHayLotes()
    {
        // Arrange
        var ids = new List<int> { 1, 2, 3 };
        var lotesVacio = new List<Lote>();

        _mockRepo.Setup(r => r.ObtenerLotesPorIDListaAsync(ids, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lotesVacio);

        // Act
        var result = await _service.ObtenerLotesPorIDLista(ids, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

}
