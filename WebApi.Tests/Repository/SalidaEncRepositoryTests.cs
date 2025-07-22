using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modelo.Entidades;
using Persistencia;
using Persistencia.Repositorios;

namespace WebApi.Tests.Repository;

[TestFixture]
public class SalidaEncRepositoryTests
{
    private BackendContext _context = null!;
    private SalidaEncRepository _repository = null!;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<BackendContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new BackendContext(options);

        var producto = new Producto
        {
            ProductoID = 100,
            Descripcion = "Producto 1",
            Estado = "A"
        };
        _context.Productos.Add(producto);

        var sucursal = new Sucursal
        {
            SucursalID = 1,
            Descripcion = "Sucursal",
            Direccion = "Dirección",
            Estado = "A"
        };
        _context.Sucursales.Add(sucursal);

        var lote = new Lote
        {
            LoteID = 10,
            ProductoID = producto.ProductoID,
            FechaVencimiento = new DateOnly(2030, 1, 1),
            Costo = 20,
            Cantidad = 100,
            CampoConcurrencia = new byte[] { 1 },
            Producto = producto
        };
        _context.Lotes.Add(lote);

        _context.SaveChanges();

        var logger = new LoggerFactory().CreateLogger<SalidaEncRepository>();
        _repository = new SalidaEncRepository(_context, logger,false);
    }

    [TearDown]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task InsertarAsync_DebeInsertarSalidaEncCorrectamente()
    {
        // Arrange
        var salida = new SalidaEnc
        {
            SucursalID = 1,
            UsuarioID = "usuario_test",
            Estado = "E",
            Fecha = DateTime.UtcNow,
            SalidaDets = new List<SalidaDet>
            {
                new SalidaDet
                {
                    LoteID = 10,
                    Cantidad = 5,
                    Costo = 20,
                    Lote = (await _context.Lotes.FindAsync(10))!
                }
            }
        };

        // Act
        var resultado = await _repository.InsertarAsync(salida, CancellationToken.None);

        // Assert
        Assert.That(resultado, Is.True);
        var insertado = await _context.SalidaEncs.Include(se => se.SalidaDets).FirstOrDefaultAsync();
        Assert.That(insertado, Is.Not.Null);
        Assert.That(insertado!.SalidaDets.Count, Is.EqualTo(1));
        Assert.That(insertado.SalidaDets.First().Cantidad, Is.EqualTo(5));
    }
}