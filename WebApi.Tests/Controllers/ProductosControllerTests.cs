using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using WebApi.Controllers;
using Aplicacion.Tablas.Productos.DTOProductos;
using Aplicacion.Core;
using static Aplicacion.Tablas.Productos.GetProducto.GetProductoQuery;
using static Aplicacion.Tablas.Productos.GetProductosActivos.GetProductosActivos;

namespace WebApi.Tests.Controllers
{
    public class ProductosControllerTests
    {
        private Mock<ISender> _mockSender;
        private ProductosController _controller;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _mockSender = new Mock<ISender>();
            _controller = new ProductosController(_mockSender.Object);
            _fixture = new Fixture();
        }

        [Test]
        public async Task ProductoGet_DevuelveOk_SiProductoExiste()
        {
            // Arrange
            var productoResponse = _fixture.Create<ProductoResponse>();
            var resultado = Result<ProductoResponse>.Success(productoResponse);

            _mockSender
                .Setup(x => x.Send(It.IsAny<GetProductoQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultado);

            // Act
            var response = await _controller.ProductoGet(1, CancellationToken.None);

            // Assert
            Assert.That(response.Result, Is.InstanceOf<OkObjectResult>());
            var ok = response.Result as OkObjectResult;
            Assert.That(ok?.Value, Is.EqualTo(productoResponse));
        }

        [Test]
        public async Task ProductoGet_DevuelveNotFound_SiProductoNoExiste()
        {
            // Arrange
            var resultado = Result<ProductoResponse>.Failure("No encontrado", HttpStatusCode.NotFound);

            _mockSender
                .Setup(x => x.Send(It.IsAny<GetProductoQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultado);

            // Act
            var response = await _controller.ProductoGet(999, CancellationToken.None);

            // Assert
            var statusResult = response.Result as ObjectResult;
            Assert.That(statusResult, Is.Not.Null);
            Assert.That(statusResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task GetProductosActivos_DevuelveOk_SiExitoso()
        {
            // Arrange
            var productos = _fixture.Create<List<ProductoResponse>>();
            var resultado = Result<List<ProductoResponse>>.Success(productos);

            _mockSender
                .Setup(x => x.Send(It.IsAny<GetProductosActivasQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultado);

            // Act
            var response = await _controller.GetProductosActivos(CancellationToken.None);

            // Assert
            Assert.That(response.Result, Is.InstanceOf<OkObjectResult>());
            var ok = response.Result as OkObjectResult;
            Assert.That(ok?.Value, Is.EqualTo(productos));
        }

        [Test]
        public async Task GetProductosActivos_DevuelveError_SiFalla()
        {
            // Arrange
            var resultado = Result<List<ProductoResponse>>.Failure("Error interno", HttpStatusCode.InternalServerError);

            _mockSender
                .Setup(x => x.Send(It.IsAny<GetProductosActivasQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultado);

            // Act
            var response = await _controller.GetProductosActivos(CancellationToken.None);

            // Assert
            var statusResult = response.Result as ObjectResult;
            Assert.That(statusResult, Is.Not.Null);
            Assert.That(statusResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
        }
    }
}