using Microsoft.EntityFrameworkCore;
using Modelo.Entidades;
using Persistencia;
using Persistencia.Repositorios;

namespace WebApi.Tests.Repository;

public class LoteRepositoryTests
{
    private BackendContext _context;
    private LoteRepository _repository;
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<BackendContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new BackendContext(options);
        _context.Database.EnsureCreated();
        _repository = new LoteRepository(_context);
    }
    [TearDown]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
    [Test]
    public async Task ObtenerLotesDisponiblesOrdenadosAsync_DevuelveSoloLotesDisponiblesDelProducto()
    {
        // Arrange
        _context.Lotes!.AddRange(
            new Lote { LoteID = 1, ProductoID = 10, Cantidad = 5, FechaVencimiento = DateOnly.FromDateTime(DateTime.Today.AddDays(5)), CampoConcurrencia = new byte[] { 1 } },
            new Lote { LoteID = 2, ProductoID = 10, Cantidad = 0, FechaVencimiento = DateOnly.FromDateTime(DateTime.Today.AddDays(10)), CampoConcurrencia = new byte[] { 1 } },
            new Lote { LoteID = 3, ProductoID = 20, Cantidad = 8, FechaVencimiento = DateOnly.FromDateTime(DateTime.Today.AddDays(2)), CampoConcurrencia = new byte[] { 1 } }
        );
        await _context.SaveChangesAsync();

        // Act
        var resultado = await _repository.ObtenerLotesDisponiblesOrdenadosAsync(10, CancellationToken.None);

        // Assert
        Assert.That(resultado.Count, Is.EqualTo(1));
        Assert.That(resultado[0].LoteID, Is.EqualTo(1));
    }
    [Test]
    public async Task ObtenerLotesDisponiblesOrdenadosAsync_DevuelveLotesOrdenadosPorVencimiento()
    {
        // Arrange
        _context.Lotes!.AddRange(
            new Lote { LoteID = 1, ProductoID = 10, Cantidad = 5, FechaVencimiento = DateOnly.FromDateTime(new DateTime(2025, 12, 1)), CampoConcurrencia = new byte[] { 1 } },
            new Lote { LoteID = 2, ProductoID = 10, Cantidad = 3, FechaVencimiento = DateOnly.FromDateTime(new DateTime(2025, 10, 1)), CampoConcurrencia = new byte[] { 1 } },
            new Lote { LoteID = 3, ProductoID = 10, Cantidad = 7, FechaVencimiento = DateOnly.FromDateTime(new DateTime(2025, 11, 1)), CampoConcurrencia = new byte[] { 1 } }
        );
        await _context.SaveChangesAsync();

        // Act
        var resultado = await _repository.ObtenerLotesDisponiblesOrdenadosAsync(10, CancellationToken.None);

        // Assert
        Assert.That(resultado.Select(l => l.LoteID), Is.EqualTo(new[] { 2, 3, 1 }));
    }
    [Test]
    public async Task ObtenerLotesDisponiblesOrdenadosAsync_DevuelveVacio_SiNoHayLotesDelProducto()
    {
        // Arrange
        _context.Lotes!.Add(new Lote { LoteID = 1, ProductoID = 99, Cantidad = 5, FechaVencimiento = DateOnly.FromDateTime(DateTime.Today), CampoConcurrencia = new byte[] { 1 } });
        await _context.SaveChangesAsync();

        // Act
        var resultado = await _repository.ObtenerLotesDisponiblesOrdenadosAsync(10, CancellationToken.None);

        // Assert
        Assert.That(resultado, Is.Empty);
    }
    [Test]
    public async Task ObtenerLotesDisponiblesOrdenadosAsync_DevuelveVacio_SiTodosEstanAgotados()
    {
        // Arrange
        _context.Lotes!.AddRange(
            new Lote { LoteID = 1, ProductoID = 10, Cantidad = 0, FechaVencimiento = DateOnly.FromDateTime(DateTime.Today), CampoConcurrencia = new byte[] { 1 } },
            new Lote { LoteID = 2, ProductoID = 10, Cantidad = 0, FechaVencimiento = DateOnly.FromDateTime(DateTime.Today.AddDays(1)), CampoConcurrencia = new byte[] { 1 } }
        );
        await _context.SaveChangesAsync();

        // Act
        var resultado = await _repository.ObtenerLotesDisponiblesOrdenadosAsync(10, CancellationToken.None);

        // Assert
        Assert.That(resultado, Is.Empty);
    }
    [Test]
    public async Task ObtenerLotesPorIDListaAsync_DevuelveLotesConIDsEspecificados()
    {
        // Arrange
        _context.Lotes!.AddRange(
            new Lote { LoteID = 1, ProductoID = 1, Cantidad = 5, FechaVencimiento = DateOnly.FromDateTime(DateTime.Today), CampoConcurrencia = new byte[] { 1 } },
            new Lote { LoteID = 2, ProductoID = 1, Cantidad = 3, FechaVencimiento = DateOnly.FromDateTime(DateTime.Today.AddDays(5)), CampoConcurrencia = new byte[] { 1 } },
            new Lote { LoteID = 3, ProductoID = 1, Cantidad = 7, FechaVencimiento = DateOnly.FromDateTime(DateTime.Today.AddDays(10)), CampoConcurrencia = new byte[] { 1 } }
        );
        await _context.SaveChangesAsync();

        // Act
        var resultado = await _repository.ObtenerLotesPorIDListaAsync(new List<int> { 1, 3 }, CancellationToken.None);

        // Assert
        Assert.That(resultado.Count, Is.EqualTo(2));
        Assert.That(resultado.Any(l => l.LoteID == 1));
        Assert.That(resultado.Any(l => l.LoteID == 3));
    }
    [Test]
    public async Task ObtenerLotesPorIDListaAsync_DevuelveListaVacia_SiNoHayCoincidencias()
    {
        // Arrange
        _context.Lotes!.Add(new Lote { LoteID = 10, ProductoID = 1, Cantidad = 5, FechaVencimiento = DateOnly.FromDateTime(DateTime.Today), CampoConcurrencia = new byte[] { 1 } });
        await _context.SaveChangesAsync();

        // Act
        var resultado = await _repository.ObtenerLotesPorIDListaAsync(new List<int> { 99, 100 }, CancellationToken.None);

        // Assert
        Assert.That(resultado, Is.Empty);
    }
    [Test]
    public async Task ObtenerLotesPorIDListaAsync_DevuelveListaVacia_SiListaIDsEsVacia()
    {
        // Arrange
        _context.Lotes!.Add(new Lote { LoteID = 1, ProductoID = 1, Cantidad = 10, FechaVencimiento = DateOnly.FromDateTime(DateTime.Today), CampoConcurrencia = new byte[] { 1 } });
        await _context.SaveChangesAsync();

        // Act
        var resultado = await _repository.ObtenerLotesPorIDListaAsync(new List<int>(), CancellationToken.None);

        // Assert
        Assert.That(resultado, Is.Empty);
    }
    [Test]
    public async Task ObtenerLotesPorIDListaAsync_DevuelveTodosLosLotes_SiTodosLosIDsExisten()
    {
        // Arrange
        var lotes = new[]
        {
            new Lote { LoteID = 1, ProductoID = 1, Cantidad = 5, FechaVencimiento = DateOnly.FromDateTime(DateTime.Today), CampoConcurrencia = new byte[] { 1 } },
            new Lote { LoteID = 2, ProductoID = 1, Cantidad = 8, FechaVencimiento = DateOnly.FromDateTime(DateTime.Today.AddDays(3)), CampoConcurrencia = new byte[] { 1 } }
        };

        _context.Lotes!.AddRange(lotes);
        await _context.SaveChangesAsync();

        // Act
        var resultado = await _repository.ObtenerLotesPorIDListaAsync(new List<int> { 1, 2 }, CancellationToken.None);

        // Assert
        Assert.That(resultado.Count, Is.EqualTo(2));
    }
    [Test]
    public async Task ObtenerLotesDisponiblesParaProductoAsync_DevuelveSoloDisponiblesDelProducto()
    {
        // Arrange
        _context.Lotes!.AddRange(
            new Lote { LoteID = 1, ProductoID = 100, Cantidad = 5, FechaVencimiento = DateOnly.FromDateTime(DateTime.Today.AddDays(10)), CampoConcurrencia = new byte[] { 1 } },
            new Lote { LoteID = 2, ProductoID = 100, Cantidad = 0, FechaVencimiento = DateOnly.FromDateTime(DateTime.Today.AddDays(5)), CampoConcurrencia = new byte[] { 1 } },
            new Lote { LoteID = 3, ProductoID = 200, Cantidad = 8, FechaVencimiento = DateOnly.FromDateTime(DateTime.Today.AddDays(1)), CampoConcurrencia = new byte[] { 1 } }
        );
        await _context.SaveChangesAsync();

        // Act
        var resultado = await _repository.ObtenerLotesDisponiblesParaProductoAsync(100, CancellationToken.None);

        // Assert
        Assert.That(resultado.Count, Is.EqualTo(1));
        Assert.That(resultado[0].LoteID, Is.EqualTo(1));
    }
    [Test]
    public async Task ObtenerLotesDisponiblesParaProductoAsync_DevuelveOrdenadosPorFechaVencimiento()
    {
        // Arrange
        _context.Lotes!.AddRange(
            new Lote { LoteID = 1, ProductoID = 10, Cantidad = 10, FechaVencimiento = DateOnly.FromDateTime(new DateTime(2025, 12, 1)), CampoConcurrencia = new byte[] { 1 } },
            new Lote { LoteID = 2, ProductoID = 10, Cantidad = 10, FechaVencimiento = DateOnly.FromDateTime(new DateTime(2025, 10, 1)), CampoConcurrencia = new byte[] { 1 } },
            new Lote { LoteID = 3, ProductoID = 10, Cantidad = 10, FechaVencimiento = DateOnly.FromDateTime(new DateTime(2025, 11, 1)), CampoConcurrencia = new byte[] { 1 } }
        );
        await _context.SaveChangesAsync();

        // Act
        var resultado = await _repository.ObtenerLotesDisponiblesParaProductoAsync(10, CancellationToken.None);

        // Assert
        Assert.That(resultado.Select(l => l.LoteID), Is.EqualTo(new[] { 2, 3, 1 }));
    }
    [Test]
    public async Task ObtenerLotesDisponiblesParaProductoAsync_DevuelveVacio_SiNoHayDisponibles()
    {
        // Arrange
        _context.Lotes!.AddRange(
            new Lote { LoteID = 1, ProductoID = 10, Cantidad = 0, FechaVencimiento = DateOnly.FromDateTime(DateTime.Today), CampoConcurrencia = new byte[] { 1 } },
            new Lote { LoteID = 2, ProductoID = 10, Cantidad = 0, FechaVencimiento = DateOnly.FromDateTime(DateTime.Today.AddDays(1)), CampoConcurrencia = new byte[] { 1 } }
        );
        await _context.SaveChangesAsync();

        // Act
        var resultado = await _repository.ObtenerLotesDisponiblesParaProductoAsync(10, CancellationToken.None);

        // Assert
        Assert.That(resultado, Is.Empty);
    }
    [Test]
    public async Task ObtenerLotesDisponiblesParaProductoAsync_DevuelveVacio_SiNoHayLotesDelProducto()
    {
        // Arrange
        _context.Lotes!.Add(new Lote
        {
            LoteID = 1,
            ProductoID = 999,
            Cantidad = 10,
            FechaVencimiento = DateOnly.FromDateTime(DateTime.Today),
            CampoConcurrencia = new byte[] { 1 }
        });
        await _context.SaveChangesAsync();

        // Act
        var resultado = await _repository.ObtenerLotesDisponiblesParaProductoAsync(123, CancellationToken.None);

        // Assert
        Assert.That(resultado, Is.Empty);
    }

}
