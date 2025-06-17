using Microsoft.EntityFrameworkCore;
using Modelo.Entidades;
using Persistencia;
using Persistencia.Repositorios;

namespace WebApi.Tests.Repository;

public class ProductoRepositoryTests
{
    private BackendContext _context;
    private ProductoRepository _repository;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<BackendContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new BackendContext(options);
        _context.Database.EnsureCreated();
        _repository = new ProductoRepository(_context);
    }

    [TearDown]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task ObtenerPorIDAsync_DevuelveProducto_SiExiste()
    {
        // Arrange
        var producto = new Producto { ProductoID = 1, Descripcion = "Zapato", Estado = "A" };
        _context.Productos!.Add(producto);
        await _context.SaveChangesAsync();

        // Act
        var resultado = await _repository.ObtenerPorIDAsync(1, CancellationToken.None);

        // Assert
        Assert.That(resultado, Is.Not.Null);
        Assert.That(resultado!.Descripcion, Is.EqualTo("Zapato"));
    }

    [Test]
    public async Task ObtenerProductosActivosAsync_DevuelveSoloActivos()
    {
        // Arrange
        _context.Productos!.AddRange(
            new Producto { ProductoID = 1, Descripcion = "Zapato A", Estado = "A" },
            new Producto { ProductoID = 2, Descripcion = "Zapato B", Estado = "I" }
        );
        await _context.SaveChangesAsync();

        // Act
        var activos = await _repository.ObtenerProductosActivosAsync(CancellationToken.None);

        // Assert
        Assert.That(activos.Count, Is.EqualTo(1));
        Assert.That(activos[0].Descripcion, Is.EqualTo("Zapato A"));
    }

    [Test]
    public async Task ObtenerInventarioDisponibleAsync_DevuelveSumaCorrecta()
    {
        // Arrange
         _context.Productos!.AddRange(
            new Producto { ProductoID = 1, Descripcion = "Producto 1", Estado = "A" },
            new Producto { ProductoID = 2, Descripcion = "Producto 2", Estado = "A" }
        );

        _context.Lotes!.AddRange(
            new Lote
            {
                LoteID = 1,
                ProductoID = 1,
                Cantidad = 10,
                FechaVencimiento = DateOnly.FromDateTime(DateTime.Today.AddMonths(6)),
                Costo = 20.0m,
                CampoConcurrencia = new byte[8]
            },
            new Lote
            {
                LoteID = 2,
                ProductoID = 1,
                Cantidad = 15,
                FechaVencimiento = DateOnly.FromDateTime(DateTime.Today.AddMonths(3)),
                Costo = 22.5m,
                CampoConcurrencia = new byte[8]
            },
            new Lote
            {
                LoteID = 3,
                ProductoID = 2,
                Cantidad = 5,
                FechaVencimiento = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)),
                Costo = 18.0m,
                CampoConcurrencia = new byte[8]
            }
        );
        await _context.SaveChangesAsync();

        // Act
        var disponible = await _repository.ObtenerInventarioDisponibleAsync(1, CancellationToken.None);

        // Assert
        Assert.That(disponible, Is.EqualTo(25));
    }
}
