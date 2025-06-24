using System.Net;
using Aplicacion.Core;
using Aplicacion.Interface;
using Aplicacion.Tablas.Salidas.SalidaCreate;
using Aplicacion.Tablas.Salidas.SalidasResponse;
using AutoFixture;
using Modelo.Entidades;
using Moq;
using WebApi.Tests.Helper;

namespace WebApi.Tests.Tablas.Salidas;

public class SalidaEncCreateCommandHandlerTests
{
    private Fixture _fixture;
    private Mock<ISalidaService> _salidaServiceMock;
    private Mock<IDistribucionService> _distribucionServiceMock;
    private Mock<IRestriccionSalidaService> _restriccionSalidaServiceMock;
    private Mock<IUsuarioService> _usuarioServiceMock;
    private Mock<ISucursalService> _sucursalServiceMock;
    private Mock<ISalidaDetListBuilder> _salidaDetListBuilderMock;
    private SalidaEncCreateCommand.SalidaEncCreateCommandHandler _handler;
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
        _salidaServiceMock = new Mock<ISalidaService>();
        _distribucionServiceMock = new Mock<IDistribucionService>();
        _restriccionSalidaServiceMock = new Mock<IRestriccionSalidaService>();
        _usuarioServiceMock = new Mock<IUsuarioService>();
        _sucursalServiceMock = new Mock<ISucursalService>();
        _salidaDetListBuilderMock = new Mock<ISalidaDetListBuilder>();

        _handler = new SalidaEncCreateCommand.SalidaEncCreateCommandHandler(
            _salidaServiceMock.Object,
            _distribucionServiceMock.Object,
            _restriccionSalidaServiceMock.Object,
            _usuarioServiceMock.Object,
            _sucursalServiceMock.Object,
            _salidaDetListBuilderMock.Object
        );
    }
    private SalidaEncCreateCommand.SalidaEncCreateCommandRequest CrearRequest()
    {
        var detalleRequest = _fixture.CreateMany<SalidaDetRequest>(2).ToList();
        var request = new SalidaEncCreateCommand.SalidaEncCreateCommandRequest(new SalidaEncCreateRequest
        {
            SucursalID = 1,
            SalidasDetalle = detalleRequest
        });
        return request;
    }

    [Test]
    public async Task Handle_RetornaOk_CuandoTodasLasValidacionesSonOK()
    {
        // Arrange
        var request = CrearRequest();

        var usuario = _fixture.Build<Usuario>().With(u => u.Id, "99").Create();
        _usuarioServiceMock.Setup(s => s.ObtenerUsuarioActualAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Usuario>.Success(usuario));

        var sucursal = _fixture.Create<Sucursal>();
        _sucursalServiceMock.Setup(s => s.ObtenerSucursalPorID(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Sucursal>.Success(sucursal));

        var distribucion = new DistribucionResultado
        {
            LotesDetalle = _fixture.CreateMany<Lote>(3).ToList(),
            LotesValidos = new List<LoteCantidadListado>
            {
                new() { LoteID = 1, Cantidad = 5 }
            }
        };
        _distribucionServiceMock.Setup(d => d.ObtenerDistribucionAsync(It.IsAny<List<SalidaDetRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(distribucion);

        var construccionResultado = new SalidaDetConstruccionResultado
        {
            SalidasDetalles = _fixture.CreateMany<SalidaDet>(2).ToList(),
            Total = 100m
        };
        _salidaDetListBuilderMock.Setup(b => b.Construir(
            request.salidaEncCreateRequest.SalidasDetalle,
            distribucion.LotesDetalle,
            distribucion.LotesValidos,
            It.IsAny<SalidaEnc>()))
            .Returns(Result<SalidaDetConstruccionResultado>.Success(construccionResultado));

        _restriccionSalidaServiceMock.Setup(r => r.ValidarLimiteSucursal(It.IsAny<int>(), 100m, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        _salidaServiceMock.Setup(s => s.RegistrarSalida(It.IsAny<SalidaEnc>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<int>.Success(1234));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(1234));

    }
    [Test]
    public async Task Handle_BrindaError_CuandoUsuarioNoEncontrado()
    {
        var request = CrearRequest();

        _usuarioServiceMock.Setup(s => s.ObtenerUsuarioActualAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Usuario>.Failure("Usuario no encontrado", HttpStatusCode.Unauthorized));

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("Usuario no encontrado"));
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
    [Test]
    public async Task Handle_BrindaError_CuandoSucursalNoEncontrada()
    {
        var request = CrearRequest();

        _usuarioServiceMock.Setup(s => s.ObtenerUsuarioActualAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Usuario>.Success(new Usuario { Id = "1" }));

        _sucursalServiceMock.Setup(s => s.ObtenerSucursalPorID(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Sucursal>.Failure("Sucursal no encontrada", HttpStatusCode.NotFound));

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("Sucursal no encontrada"));
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
    [Test]
    public async Task Handle_BrindaError_CuandoConstruccionDetalleFalla()
    {
        var request = CrearRequest();

        _usuarioServiceMock.Setup(x => x.ObtenerUsuarioActualAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Usuario>.Success(new Usuario { Id = "1" }));

        _sucursalServiceMock.Setup(x => x.ObtenerSucursalPorID(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Sucursal>.Success(new Sucursal()));

        _distribucionServiceMock.Setup(x => x.ObtenerDistribucionAsync(It.IsAny<List<SalidaDetRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DistribucionResultado());

        _salidaDetListBuilderMock.Setup(x => x.Construir(
            It.IsAny<List<SalidaDetRequest>>(),
            It.IsAny<List<Lote>>(),
            It.IsAny<List<LoteCantidadListado>>(),
            It.IsAny<SalidaEnc>()
        )).Returns(Result<SalidaDetConstruccionResultado>.Failure("Error al construir detalles", HttpStatusCode.BadRequest));

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("Error al construir detalles"));
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
    [Test]
    public async Task Handle_BrindaError_CuandoRestriccionSalidaEsInvalida()
    {
        var request = CrearRequest();

        _usuarioServiceMock.Setup(x => x.ObtenerUsuarioActualAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Usuario>.Success(new Usuario { Id = "1" }));

        _sucursalServiceMock.Setup(x => x.ObtenerSucursalPorID(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Sucursal>.Success(new Sucursal()));

        _distribucionServiceMock.Setup(x => x.ObtenerDistribucionAsync(It.IsAny<List<SalidaDetRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DistribucionResultado());

        _salidaDetListBuilderMock.Setup(x => x.Construir(
            It.IsAny<List<SalidaDetRequest>>(),
            It.IsAny<List<Lote>>(),
            It.IsAny<List<LoteCantidadListado>>(),
            It.IsAny<SalidaEnc>()
        )).Returns(Result<SalidaDetConstruccionResultado>.Success(new SalidaDetConstruccionResultado
        {
            SalidasDetalles = [],
            Total = 200
        }));

        _restriccionSalidaServiceMock.Setup(x => x.ValidarLimiteSucursal(It.IsAny<int>(), 200, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("Restricción superada", HttpStatusCode.BadRequest));

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("Restricción superada"));
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
    [Test]
    public async Task Handle_BrindaError_CuandoRegistroSalidaFalla()
    {
        var request = CrearRequest();

        _usuarioServiceMock.Setup(x => x.ObtenerUsuarioActualAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Usuario>.Success(new Usuario { Id = "1" }));

        _sucursalServiceMock.Setup(x => x.ObtenerSucursalPorID(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Sucursal>.Success(new Sucursal()));

        _distribucionServiceMock.Setup(x => x.ObtenerDistribucionAsync(It.IsAny<List<SalidaDetRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DistribucionResultado());

        _salidaDetListBuilderMock.Setup(x => x.Construir(
            It.IsAny<List<SalidaDetRequest>>(),
            It.IsAny<List<Lote>>(),
            It.IsAny<List<LoteCantidadListado>>(),
            It.IsAny<SalidaEnc>()
        )).Returns(Result<SalidaDetConstruccionResultado>.Success(new SalidaDetConstruccionResultado
        {
            SalidasDetalles = [],
            Total = 50
        }));

        _restriccionSalidaServiceMock.Setup(x => x.ValidarLimiteSucursal(It.IsAny<int>(), 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        _salidaServiceMock.Setup(x => x.RegistrarSalida(It.IsAny<SalidaEnc>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<int>.Failure("Error al registrar", HttpStatusCode.InternalServerError));

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("Error al registrar"));
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }

}
