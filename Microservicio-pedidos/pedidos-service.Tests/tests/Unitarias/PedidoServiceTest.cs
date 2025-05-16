using Moq;
using pedidos_service.Application.DTOs;
using pedidos_service.Application.Services;
using pedidos_service.Domain.Entities;
using pedidos_service.Domain.Repositories;
using pedidos_service.Domain.Services;
using pedidos_service.Domain.ValueObjects;
using Xunit;

public class PedidoServiceTests
{
    private readonly Mock<IPedidoRepository> _pedidoRepositoryMock;
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly Mock<ICreacionPedidoService> _creacionPedidoServiceMock;
    private readonly PedidoService _pedidoService;

    public PedidoServiceTests()
    {
        _pedidoRepositoryMock = new Mock<IPedidoRepository>();
        _clienteRepositoryMock = new Mock<IClienteRepository>();
        _creacionPedidoServiceMock = new Mock<ICreacionPedidoService>();

        _pedidoService = new PedidoService(
            _pedidoRepositoryMock.Object,
            _clienteRepositoryMock.Object,
            _creacionPedidoServiceMock.Object
        );
    }

    [Fact]
    public async Task CrearPedidoAsync_DeberiaCrearPedido_CuandoClienteExiste()
    {
        // Arrange
        var clienteId = "123";
        var direccion = new DireccionEntrega("Calle 33", "4735", "medellin", "050016", "buenos aires");
        var cliente = new Cliente("John Estiven", "johnrestrepo@correo.itm.edu.co", "312312312", direccion);

        var crearPedidoDTO = new CrearPedidoDTO
        {
            ClienteId = clienteId,
            DireccionEntrega = new DireccionEntregaDTO
            {
                Calle = direccion.Calle,
                Numero = direccion.Numero,
                Ciudad = direccion.Ciudad,
                CodigoPostal = direccion.CodigoPostal,
                Referencia = direccion.Referencia
            },
            Items = new List<ItemPedidoDTO>
            {
                new ItemPedidoDTO { ProductoId = "prod1", Cantidad = 2, PrecioUnitario = 5000 }
            }
        };

        var pedido = new Pedido(clienteId, direccion);
        pedido.AgregarItem("prod1", 2, 5000);

        _clienteRepositoryMock.Setup(repo => repo.GetByIdAsync(clienteId))
            .ReturnsAsync(cliente);

        _creacionPedidoServiceMock.Setup(service => service.CrearPedidoAsync(
            clienteId,
            It.IsAny<DireccionEntrega>(),
            It.IsAny<List<(string, int, decimal)>>()))
            .ReturnsAsync(pedido);

        _pedidoRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Pedido>()))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _pedidoService.CrearPedidoAsync(crearPedidoDTO);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(clienteId, resultado.ClienteId);
        Assert.Single(resultado.Items);
        _pedidoRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Pedido>()), Times.Once);
    }

    [Fact]
    public async Task CrearPedidoAsync_DeberiaLanzarExcepcion_CuandoClienteNoExiste()
    {
        // Arrange
        var clienteId = "123";
        var crearPedidoDTO = new CrearPedidoDTO
        {
            ClienteId = clienteId,
            DireccionEntrega = new DireccionEntregaDTO
            {
                Calle = "Calle 33",
                Numero = "4735",
                Ciudad = "medellin",
                CodigoPostal = "050016",
                Referencia = "buenos aires"
            },
            Items = new List<ItemPedidoDTO>
            {
                new ItemPedidoDTO { ProductoId = "prod1", Cantidad = 2, PrecioUnitario = 5000 }
            }
        };

        _clienteRepositoryMock.Setup(repo => repo.GetByIdAsync(clienteId))
            .ReturnsAsync((Cliente)null);

        // Act Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _pedidoService.CrearPedidoAsync(crearPedidoDTO));
    }

    [Fact]
    public async Task GetPedidoByIdAsync_DeberiaRetornarPedido_CuandoExiste()
    {
        // Arrange
        var pedidoId = "p1";
        var pedido = new Pedido("c1", new DireccionEntrega("cra33", "3745", "medfellin", "05000", "buenos aires"));
        _pedidoRepositoryMock.Setup(r => r.GetByIdAsync(pedidoId)).ReturnsAsync(pedido);

        // Act
        var resultado = await _pedidoService.GetPedidoByIdAsync(pedidoId);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("c1", resultado.ClienteId);
    }

    [Fact]
    public async Task GetPedidoByIdAsync_DeberiaLanzarExcepcion_CuandoNoExiste()
    {
        // Arrange
        var pedidoId = "noexiste";
        _pedidoRepositoryMock.Setup(r => r.GetByIdAsync(pedidoId)).ReturnsAsync((Pedido)null);

        // Act Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _pedidoService.GetPedidoByIdAsync(pedidoId));
    }
    [Fact]
    public async Task GetPedidosByEstadoAsync_DeberiaRetornarPedidos_CuandoEstadoEsValido()
    {
        // Arrange
        var estado = "CONFIRMADO";
        var pedidos = new List<Pedido>
    {
        new Pedido("john e", new DireccionEntrega("cra33", "4725", "medellin", "0500", "Buenos aires"))
    };
        _pedidoRepositoryMock.Setup(r => r.GetByEstadoAsync(EstadoPedido.CONFIRMADO)).ReturnsAsync(pedidos);

        // Act
        var resultado = await _pedidoService.GetPedidosByEstadoAsync(estado);

        // Assert
        Assert.Single(resultado);
    }

    [Fact]
    public async Task GetPedidosByEstadoAsync_DeberiaLanzarExcepcion_CuandoEstadoEsInvalido()
    {
        // Arrange
        var estadoInvalido = "ESTADONOVALIDO";

        // Act Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _pedidoService.GetPedidosByEstadoAsync(estadoInvalido));
    }

    [Theory]
    [InlineData("EN_PREPARACION", EstadoPedido.CONFIRMADO)]
    [InlineData("LISTO", EstadoPedido.EN_PREPARACION)]
    [InlineData("ENTREGADO", EstadoPedido.LISTO)]
    public async Task ActualizarEstadoPedidoAsync_DeberiaActualizarEstado_CuandoEstadoEsValido(string nuevoEstado, EstadoPedido estadoActual)
    {
        // Arrange
        var pedidoId = "p1";
        var pedido = new Pedido("c1", new DireccionEntrega("cra", "1", "med", "123", "aaaaa"));
        _pedidoRepositoryMock.Setup(r => r.GetByIdAsync(pedidoId)).ReturnsAsync(pedido);
        _pedidoRepositoryMock.Setup(r => r.UpdateAsync(pedido)).Returns(Task.CompletedTask);

        switch (estadoActual)
        {
            case EstadoPedido.CONFIRMADO:
                pedido.ConfirmarPedido();
                break;
            case EstadoPedido.EN_PREPARACION:
                pedido.ConfirmarPedido();
                pedido.MarcarEnPreparacion();
                break;
            case EstadoPedido.LISTO:
                pedido.ConfirmarPedido();
                pedido.MarcarEnPreparacion();
                pedido.MarcarListo();
                break;
        }

        // Act
        var resultado = await _pedidoService.ActualizarEstadoPedidoAsync(pedidoId, nuevoEstado);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(nuevoEstado, pedido.Estado.ToString());
    }

    [Fact]
    public async Task ActualizarEstadoPedidoAsync_DeberiaLanzarExcepcion_CuandoPedidoNoExiste()
    {
        // Arrange
        var pedidoId = "noexiste";
        _pedidoRepositoryMock.Setup(r => r.GetByIdAsync(pedidoId)).ReturnsAsync((Pedido)null);

        // Act Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _pedidoService.ActualizarEstadoPedidoAsync(pedidoId, "CONFIRMADO"));
    }

    [Fact]
    public async Task ActualizarEstadoPedidoAsync_DeberiaLanzarExcepcion_CuandoEstadoNoValido()
    {
        // Arrange
        var pedidoId = "p1";
        var pedido = new Pedido("john e", new DireccionEntrega("cr33", "4735", "medellin", "05000", "Buenos"));
        _pedidoRepositoryMock.Setup(r => r.GetByIdAsync(pedidoId)).ReturnsAsync(pedido);

        // Act Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _pedidoService.ActualizarEstadoPedidoAsync(pedidoId, "INVALIDO"));
    }
}