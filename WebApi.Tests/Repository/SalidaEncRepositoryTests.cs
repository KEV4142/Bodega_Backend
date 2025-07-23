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
        _repository = new SalidaEncRepository(_context, logger, false);
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

    [Test]
    public async Task ObtenerPorIDAsync_DebeRetornarNullCuandoNoExiste()
    {
        // Act
        var resultado = await _repository.ObtenerPorIDAsync(999, CancellationToken.None);

        // Assert
        Assert.That(resultado, Is.Null);
    }
    [Test]
    public async Task ObtenerPorIDAsync_DebeRetornarSalidaEncCuandoExiste()
    {
        // Arrange
        var salida = new SalidaEnc
        {
            SalidaID = 200,
            SucursalID = 1,
            UsuarioID = "usuario prueba",
            Estado = "E",
            Fecha = DateTime.UtcNow
        };
        _context.SalidaEncs!.Add(salida);
        await _context.SaveChangesAsync();

        // Act
        var resultado = await _repository.ObtenerPorIDAsync(200, CancellationToken.None);

        // Assert
        Assert.That(resultado, Is.Not.Null);
        Assert.That(resultado!.SalidaID, Is.EqualTo(200));
        Assert.That(resultado.UsuarioID, Is.EqualTo("usuario prueba"));
    }
    [Test]
    public async Task ActualizarEstadoAsync_DebeActualizarCorrectamente()
    {
        // Arrange
        var salida = new SalidaEnc
        {
            SalidaID = 300,
            SucursalID = 1,
            UsuarioID = "usuario abc ID",
            Estado = "E",
            Fecha = DateTime.UtcNow
        };
        _context.SalidaEncs!.Add(salida);
        await _context.SaveChangesAsync();

        // Act
        var resultado = await _repository.ActualizarEstadoAsync(salida, "R", "usuario Recibe", CancellationToken.None);

        // Assert
        Assert.That(resultado, Is.True);

        var actualizado = await _context.SalidaEncs.FirstOrDefaultAsync(se => se.SalidaID == 300);
        Assert.That(actualizado, Is.Not.Null);
        Assert.That(actualizado!.Estado, Is.EqualTo("R"));
        Assert.That(actualizado.UsuarioRecibe, Is.EqualTo("usuario Recibe"));
        Assert.That(actualizado.FechaRecibido, Is.Not.Null);
        Assert.That(actualizado.FechaRecibido.Value.Date, Is.EqualTo(DateTime.Now.Date));
    }
    [Test]
    public async Task ObtenerTotalCostoPendientePorSucursalAsync_DebeRetornarSumaCorrecta()
    {
        // Arrange
        var salida1 = new SalidaEnc
        {
            SalidaID = 500,
            SucursalID = 1,
            UsuarioID = "usuario Prueba",
            Estado = "E",
            Fecha = DateTime.UtcNow,
            SalidaDets = new List<SalidaDet>
        {
            new SalidaDet
            {
                LoteID = 10,
                Cantidad = 3,
                Costo = 30,
                Lote = (await _context.Lotes.FindAsync(10))!
            }
        }
        };

        var salida2 = new SalidaEnc
        {
            SalidaID = 501,
            SucursalID = 1,
            UsuarioID = "usuario Prueba",
            Estado = "R",
            Fecha = DateTime.UtcNow,
            SalidaDets = new List<SalidaDet>
        {
            new SalidaDet
            {
                LoteID = 10,
                Cantidad = 3,
                Costo = 20,
                Lote = (await _context.Lotes.FindAsync(10))!
            }
        }
        };

        _context.SalidaEncs.AddRange(salida1, salida2);
        await _context.SaveChangesAsync();

        // Act
        var total = await _repository.ObtenerTotalCostoPendientePorSucursalAsync(1, CancellationToken.None);

        // Assert
        Assert.That(total, Is.EqualTo(90));
    }
    [Test]
    public async Task ObtenerTotalCostoPendientePorSucursalAsync_DebeRetornarCero_SiNoHaySalidasConEstadoE()
    {
        var salida = new SalidaEnc
        {
            SalidaID = 600,
            SucursalID = 1,
            UsuarioID = "usuario Prueba ABC",
            Estado = "R",
            Fecha = DateTime.UtcNow,
            SalidaDets = new List<SalidaDet>
        {
            new SalidaDet
            {
                LoteID = 10,
                Cantidad = 3,
                Costo = 20,
                Lote = (await _context.Lotes.FindAsync(10))!
            }
        }
        };

        _context.SalidaEncs.Add(salida);
        await _context.SaveChangesAsync();

        // Act
        var total = await _repository.ObtenerTotalCostoPendientePorSucursalAsync(1, CancellationToken.None);

        // Assert
        Assert.That(total, Is.EqualTo(0m));
    }
    [Test]
    public async Task ObtenerTotalCostoPendientePorSucursalAsync_DebeRetornarCero_SiNoHaySalidasParaSucursal()
    {
        // Act
        var total = await _repository.ObtenerTotalCostoPendientePorSucursalAsync(999, CancellationToken.None);

        // Assert
        Assert.That(total, Is.EqualTo(0m));
    }


}