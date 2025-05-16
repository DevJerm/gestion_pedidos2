using pedidos_service.Application.Services;
using pedidos_service.Domain.ValueObjects;
using Xunit;

public class CreacionPedidoServiceTests
{
    private readonly CreacionPedidoService _service;

    public CreacionPedidoServiceTests()
    {
        _service = new CreacionPedidoService();
    }

    [Fact]
    public async Task CrearPedidoAsync_ValidData_ReturnsPedido()
    {
        // Arrange
        var clienteId = "12345";
        var direccion = new DireccionEntrega( "Calle Principal", "123", "Ciudad", "12345", "Cerca del parque");
        var items = new List<(string productoId, int cantidad, decimal precioUnitario)>
        {
            ("prod-1", 2, 10),
            ("prod-2", 1, 15)
        };

        // Act
        var pedido = await _service.CrearPedidoAsync(clienteId, direccion, items);

        // Assert
        Assert.NotNull(pedido);
        Assert.Equal(clienteId, pedido.ClienteId);
        Assert.Equal(direccion, pedido.DireccionEntrega);
        Assert.Equal(2, pedido.Items.Count);
    }

    [Fact]
    public async Task CrearPedidoAsync_NoItems_ThrowsArgumentException()
    {
        // Arrange
        var clienteId = "12345";
        var direccion = new DireccionEntrega("Calle 33", "123", "medellin", "12345", "Cerca del parque");
        var items = new List<(string productoId, int cantidad, decimal precioUnitario)>();

        // Act 
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CrearPedidoAsync(clienteId, direccion, items));
        // Assert
        Assert.Equal("Un pedido debe contener al menos un item", exception.Message);
    }

    [Fact]
    public async Task CrearPedidoAsync_CantidadCero_ThrowsArgumentException()
    {
        // Arrange
        var clienteId = "12345";
        var direccion = new DireccionEntrega("Calle 33", "123", "medellin", "12345", "Cerca del parque");
        var items = new List<(string productoId, int cantidad, decimal precioUnitario)>
        {
            ("prod-1", 0, 10000) // Cantidad inválida
        };

        // Act Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CrearPedidoAsync(clienteId, direccion, items));

        Assert.Contains("La cantidad debe ser mayor que cero", exception.Message);
    }

    [Fact]
    public async Task CrearPedidoAsync_PrecioCero_ThrowsArgumentException()
    {
        // Arrange
        var clienteId = "12345";
        var direccion = new DireccionEntrega("Calle 33", "123", "medellin", "12345", "Cerca del parque");
        var items = new List<(string productoId, int cantidad, decimal precioUnitario)>
        {
            ("prod-1", 2, 0) 
        };

        // Act  
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CrearPedidoAsync(clienteId, direccion, items));
        // Assert
        Assert.Contains("El precio debe ser mayor que cero", exception.Message);
    }
}
