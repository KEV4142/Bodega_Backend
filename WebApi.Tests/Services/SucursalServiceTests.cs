using System.Net;
using Aplicacion.Service;
using Aplicacion.Tablas.Sucursales.DTOSucursales;
using AutoFixture;
using AutoMapper;
using Modelo.Entidades;
using Modelo.Interfaces;
using Moq;
using WebApi.Tests.Helper;

namespace WebApi.Tests.Services;

public class SucursalServiceTests
{
    private Mock<ISucursalRepository> _mockRepo;
    private Mock<IMapper> _mockMapper;
    private SucursalService _service;
    private Fixture _fixture;
    [SetUp]
    public void Setup()
    {
        _mockRepo = new Mock<ISucursalRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new SucursalService(_mockRepo.Object, _mockMapper.Object);
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
    public async Task ObtenerSucursalPorIDResponse_DevuelveSuccess_SiExiste()
    {
        // Arrange
        var sucursal = new Sucursal
        {
            SucursalID = 1,
            Descripcion = "Prueba",
            Direccion = "Direccion de prueba",
            Estado = "A"
        };

        var sucursalResponse = _fixture.Create<SucursalResponse>();

        _mockRepo
            .Setup(r => r.ObtenerPorIDAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sucursal);

        _mockMapper
            .Setup(m => m.Map<SucursalResponse>(sucursal))
            .Returns(sucursalResponse);

        // Act
        var result = await _service.ObtenerSucursalPorIDResponse(1, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(sucursalResponse));
    }
    [Test]
    public async Task ObtenerSucursalPorIDResponse_DevuelveFailure_SiNoExiste()
    {
        // Arrange
        _mockRepo
            .Setup(r => r.ObtenerPorIDAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sucursal?)null);

        // Act
        var result = await _service.ObtenerSucursalPorIDResponse(999, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        Assert.That(result.Error, Is.EqualTo("No se encontró la sucursal."));
    }
    [Test]
    public async Task ObtenerSucursalPorID_DevuelveSuccess_SiExiste()
    {
        // Arrange
        var sucursal = _fixture.Create<Sucursal>();
        var sucursalResponse = sucursal;

        _mockRepo
            .Setup(r => r.ObtenerPorIDAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sucursal);

        _mockMapper
            .Setup(m => m.Map<Sucursal>(sucursal))
            .Returns(sucursalResponse);

        // Act
        var result = await _service.ObtenerSucursalPorID(1, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(sucursalResponse));
    }
    [Test]
    public async Task ObtenerSucursalPorID_DevuelveFailure_SiNoExiste()
    {
        // Arrange
        _mockRepo
            .Setup(r => r.ObtenerPorIDAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sucursal?)null);

        // Act
        var result = await _service.ObtenerSucursalPorID(999, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        Assert.That(result.Error, Is.EqualTo("No se encontró la sucursal."));
    }
    [Test]
    public async Task ObtenerSucursalesActivos_DevuelveSuccess_SiHaySucursales()
    {
        // Arrange
        var sucursales = _fixture.CreateMany<Sucursal>(3).ToList();
        var sucursalesDTO = _fixture.CreateMany<SucursalResponse>(3).ToList();

        _mockRepo
            .Setup(r => r.ObtenerSucursalesActivasAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(sucursales);

        _mockMapper
            .Setup(m => m.Map<List<SucursalResponse>>(sucursales))
            .Returns(sucursalesDTO);

        // Act
        var result = await _service.ObtenerSucursalesActivas(CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(sucursalesDTO));
    }
    [Test]
    public async Task ObtenerSucursalesActivos_DevuelveListaVacia_SiNoHaySucursales()
    {
        // Arrange
        var sucursales = new List<Sucursal>
        {
            new Sucursal
            {
                SucursalID = 1,
                Descripcion = "Sucursal A",
                Direccion = "Direccion Sucursal A",
                Estado = "A"
            }
        };

        var sucursalDTO = new List<SucursalResponse>();

        _mockRepo
            .Setup(r => r.ObtenerSucursalesActivasAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(sucursales);

        _mockMapper
            .Setup(m => m.Map<List<SucursalResponse>>(sucursales))
            .Returns(sucursalDTO);

        // Act
        var result = await _service.ObtenerSucursalesActivas(CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Empty);
    }
}
