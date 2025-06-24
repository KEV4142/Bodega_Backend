using System.Net;
using Aplicacion.Core;
using Aplicacion.Interface;
using Aplicacion.Tablas.Salidas.SalidaUpdateEstado;
using Modelo.Entidades;
using Moq;

namespace WebApi.Tests.Tablas.Salidas;

public class SalidaUpdateEstadoCommandHandlerTests
{
    private Mock<ISalidaService> _salidaServiceMock;
    private Mock<IUsuarioService> _usuarioServiceMock;
    private SalidaUpdateEstadoCommand.SalidaUpdateEstadoCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _salidaServiceMock = new Mock<ISalidaService>();
        _usuarioServiceMock = new Mock<IUsuarioService>();

        _handler = new SalidaUpdateEstadoCommand.SalidaUpdateEstadoCommandHandler(
            _salidaServiceMock.Object,
            _usuarioServiceMock.Object
        );
    }
    private SalidaUpdateEstadoCommand.SalidaUpdateEstadoCommandRequest CrearRequest(string estado = "R")
    {
        return new(new SalidaUpdateEstadoRequest { Estado = estado }, SalidaID: 10);
    }
    [Test]
    public async Task Handle_BrindaOK_CuandoEstadoEsR()
    {
        var request = CrearRequest("R");

        _usuarioServiceMock.Setup(x => x.ObtenerUsuarioActualAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Usuario>.Success(new Usuario { Id = "99" }));

        _salidaServiceMock.Setup(x => x.ObtenerSalidaPorID(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<SalidaEnc>.Success(new SalidaEnc()));

        _salidaServiceMock.Setup(x => x.CambiarEstadoSalida(It.IsAny<SalidaEnc>(), "R", "99", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<int>.Success(1));

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(1));
    }
    [Test]
    public async Task Handle_BrindaError_CuandoUsuarioNoEncontrado()
    {
        var request = CrearRequest("R");

        _usuarioServiceMock.Setup(x => x.ObtenerUsuarioActualAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Usuario>.Failure("Usuario no encontrado", HttpStatusCode.Unauthorized));

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("Usuario no encontrado"));
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
    [Test]
    public async Task Handle_BrindaError_CuandoSalidaNoExiste()
    {
        var request = CrearRequest("R");

        _usuarioServiceMock.Setup(x => x.ObtenerUsuarioActualAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Usuario>.Success(new Usuario { Id = "99" }));

        _salidaServiceMock.Setup(x => x.ObtenerSalidaPorID(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<SalidaEnc>.Failure("Salida no existe", HttpStatusCode.NotFound));

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("Salida no existe"));
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
    [Test]
    public async Task Handle_BrindaError_CuandoFallaAlActualizarEstado()
    {
        var request = CrearRequest("R");

        _usuarioServiceMock.Setup(x => x.ObtenerUsuarioActualAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Usuario>.Success(new Usuario { Id = "99" }));

        _salidaServiceMock.Setup(x => x.ObtenerSalidaPorID(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<SalidaEnc>.Success(new SalidaEnc()));

        _salidaServiceMock.Setup(x => x.CambiarEstadoSalida(It.IsAny<SalidaEnc>(), "R", "99", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<int>.Failure("Error al actualizar", HttpStatusCode.BadRequest));

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("Error al actualizar"));
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

}
