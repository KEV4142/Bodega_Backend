using Microsoft.EntityFrameworkCore;
using Modelo.Entidades;
using Persistencia;
using Persistencia.Repositorios;

namespace WebApi.Tests.Repository;

public class SucursalRepositoryTests
{
    private BackendContext _context;
    private SucursalRepository _repository;
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<BackendContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new BackendContext(options);
        _context.Database.EnsureCreated();
        _repository = new SucursalRepository(_context);
    }
    [TearDown]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task ObtenerPorIDAsync_DevuelveSucursal_SiExiste()
    {
        // Arrange
        var sucursal = new Sucursal { SucursalID = 1, Descripcion = "Aurora", Direccion = "SPS", Estado = "A" };
        _context.Sucursales!.Add(sucursal);
        await _context.SaveChangesAsync();

        // Act
        var resultado = await _repository.ObtenerPorIDAsync(1, CancellationToken.None);

        // Assert
        Assert.That(resultado, Is.Not.Null);
        Assert.That(resultado!.Descripcion, Is.EqualTo("Aurora"));
    }
    [Test]
    public async Task ObtenerPorIDAsync_DevuelveNull_SiNoExiste()
    {
        // Act
        var resultado = await _repository.ObtenerPorIDAsync(999, CancellationToken.None);

        // Assert
        Assert.That(resultado, Is.Null);
    }
    [Test]
    public async Task ObtenerSucursalesActivosAsync_DevuelveSoloActivos()
    {
        // Arrange
        _context.Sucursales!.AddRange(
            new Sucursal { SucursalID = 1, Descripcion = "Aurora", Direccion = "SPS", Estado = "A" },
            new Sucursal { SucursalID = 2, Descripcion = "Centro", Direccion = "SPS", Estado = "I" },
            new Sucursal { SucursalID = 3, Descripcion = "Norte", Direccion = "SPS", Estado = "A" }
        );
        await _context.SaveChangesAsync();

        // Act
        var activos = await _repository.ObtenerSucursalesActivasAsync(CancellationToken.None);

        // Assert
        Assert.That(activos.Count, Is.EqualTo(2));
        Assert.That(activos[1].Descripcion, Is.EqualTo("Norte"));
    }
    [Test]
    public async Task ObtenerSucursalesActivosAsync_DevuelveListaVacia_SiNoHayActivos()
    {
        // Arrange
        _context.Sucursales!.AddRange(
            new Sucursal { SucursalID = 1, Descripcion = "Sucursal 1", Direccion = "SPS", Estado = "I" },
            new Sucursal { SucursalID = 2, Descripcion = "Sucursal 2", Direccion = "TGU", Estado = "I" }
        );
        await _context.SaveChangesAsync();

        // Act
        var activos = await _repository.ObtenerSucursalesActivasAsync(CancellationToken.None);

        // Assert
        Assert.That(activos, Is.Empty);
    }
    [Test]
    public async Task ObtenerSucursalesActivosAsync_DevuelveListaVacia_SiNoHaySucursales()
    {
        // Arrange
        // No agregamos ninguna sucursal al contexto

        // Act
        var activos = await _repository.ObtenerSucursalesActivasAsync(CancellationToken.None);

        // Assert
        Assert.That(activos, Is.Empty);
    }


}
