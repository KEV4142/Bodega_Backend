using System.Net;
using Aplicacion.Core;
using Aplicacion.Tablas.Accounts;
using Aplicacion.Tablas.Accounts.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Seguridad.Interfaces;
using WebApi.Controllers;
using static Aplicacion.Tablas.Accounts.GetCurrentUser.GetCurrentUserQuery;
using static Aplicacion.Tablas.Accounts.Login.LoginCommand;

namespace WebApi.Tests.Controllers;

public class AccountControllerTests
{
    private Mock<ISender> _senderMock = null!;
    private Mock<IUserAccessor> _userMock = null!;
    private AccountController _controller = null!;

    [SetUp]
    public void Setup()
    {
        _senderMock = new Mock<ISender>();
        _userMock = new Mock<IUserAccessor>();
        _controller = new AccountController(_senderMock.Object, _userMock.Object);
    }

    [Test]
    public async Task Login_DevuelveOk_ConTokenSiCredencialesValidas()
    {
        // Arrange
        var request = new LoginRequest { Email = "test@correo.com", Password = "1234" };
        var profile = new Profile
        {
            NombreCompleto = "Test User",
            Email = request.Email,
            Token = "jwt.token",
            Username = "testuser",
            Tipo = "Admin"
        };

        var resultado = Result<Profile>.Success(profile);

        _senderMock
            .Setup(s => s.Send(It.IsAny<LoginCommandRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultado);

        // Act
        var response = await _controller.Login(request, CancellationToken.None);

        // Assert
        Assert.That(response.Result, Is.TypeOf<OkObjectResult>());
        var ok = (OkObjectResult)response.Result!;
        var value = (Profile)ok.Value!;
        Assert.That(value.Token, Is.EqualTo("jwt.token"));
    }
    [Test]
    public async Task Login_DevuelveUnauthorized_SiCredencialesInvalidas()
    {
        // Arrange
        var request = new LoginRequest { Email = "test@correo.com", Password = "wrong" };
        var resultado = Result<Profile>.Failure("Credenciales inválidas", HttpStatusCode.Unauthorized);

        _senderMock
            .Setup(s => s.Send(It.IsAny<LoginCommandRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultado);

        // Act
        var response = await _controller.Login(request, CancellationToken.None);

        // Assert
        Assert.That(response.Result, Is.TypeOf<UnauthorizedObjectResult>());
    }
    [Test]
    public async Task Me_DevuelvePerfil_SiUsuarioAutenticado()
    {
        // Arrange
        var email = "test@correo.com";
        _userMock.Setup(u => u.GetEmail()).Returns(email);

        var profile = new Profile
        {
            NombreCompleto = "Test User",
            Email = email,
            Token = "jwt.token",
            Username = "testuser",
            Tipo = "Admin"
        };

        var resultado = Result<Profile>.Success(profile);

        _senderMock
            .Setup(s => s.Send(It.IsAny<GetCurrentUserQueryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultado);

        // Act
        var response = await _controller.Me(CancellationToken.None);

        // Assert
        Assert.That(response.Result, Is.TypeOf<OkObjectResult>());
        var ok = (OkObjectResult)response.Result!;
        var value = (Profile)ok.Value!;
        Assert.That(value.Email, Is.EqualTo(email));
    }
    [Test]
    public async Task Me_DevuelveNotFound_SiUsuarioNoExiste()
    {
        // Arrange
        var email = "no@existe.com";
        _userMock.Setup(u => u.GetEmail()).Returns(email);

        var resultado = Result<Profile>.Failure("Usuario no encontrado", HttpStatusCode.NotFound);

        _senderMock
            .Setup(s => s.Send(It.IsAny<GetCurrentUserQueryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultado);

        // Act
        var response = await _controller.Me(CancellationToken.None);

        // Assert
        var status = response.Result as ObjectResult;
        Assert.That(status!.StatusCode, Is.EqualTo(404));
    }

}
