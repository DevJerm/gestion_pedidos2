using MongoDB.Driver;
using Moq;
using pedidos_service.Domain.Entities;
using pedidos_service.Domain.ValueObjects;
using pedidos_service.Infraestructure.Persistence.MongoDB;
using pedidos_service.Infraestructure.Persistence.Repositories;
using Xunit;

namespace pedidos_service.tests.Unitarias.Repositories
{
    public class PedidoRepositoryTests
    {
        private readonly Mock<IMongoCollection<Pedido>> _mockCollection;
        private readonly Mock<IAsyncCursor<Pedido>> _mockCursor;
        private readonly PedidoRepository _repository;

        public PedidoRepositoryTests()
        {
            _mockCollection = new Mock<IMongoCollection<Pedido>>();
            _mockCursor = new Mock<IAsyncCursor<Pedido>>();

            var mockContext = new Mock<IMongoDBContext>();
            mockContext.Setup(c => c.Pedidos).Returns(_mockCollection.Object);

            _repository = new PedidoRepository(mockContext.Object);
        }

        private Pedido CrearPedidoConId(string id)
        {
            var direccion = new DireccionEntrega("Cr33", "47 35", "Medellin", "050000", "buenos");
            var pedido = new Pedido("cliente123", direccion);

            typeof(Pedido).GetProperty("Id")?.SetValue(pedido, id);

            pedido.AgregarItem("prod1", 2, 5000);
            pedido.ConfirmarPedido();

            return pedido;
        }

        [Fact]
        public async Task GetByIdAsync_RetornaPedido_CuandoExiste()
        {
            // Arrange
            var id = "p-123";
            var pedidoEsperado = CrearPedidoConId(id);

            _mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(true).ReturnsAsync(false);
            _mockCursor.SetupGet(c => c.Current).Returns(new List<Pedido> { pedidoEsperado });

            _mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Pedido>>(),
                It.IsAny<FindOptions<Pedido, Pedido>>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(_mockCursor.Object);

            // Act
            var result = await _repository.GetByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("cliente123", result.ClienteId);
        }

        [Fact]
        public async Task GetAllAsync_RetornaListaDePedidos()
        {
            // Arrange
            var direccion = new DireccionEntrega("Calle 1", "123", "Medellin", "050001", "Casa");
            var pedidos = new List<Pedido>
            {
                new Pedido("cliente1", direccion),
                new Pedido("cliente2", direccion)
            };

            _mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(true).ReturnsAsync(false);
            _mockCursor.SetupGet(c => c.Current).Returns(pedidos);

            _mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Pedido>>(),
                It.IsAny<FindOptions<Pedido, Pedido>>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(_mockCursor.Object);

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByClienteIdAsync_RetornaPedidosDelCliente()
        {
            // Arrange
            var pedido = CrearPedidoConId("p-1");

            _mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(true).ReturnsAsync(false);
            _mockCursor.SetupGet(c => c.Current).Returns(new List<Pedido> { pedido });

            _mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Pedido>>(),
                It.IsAny<FindOptions<Pedido, Pedido>>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(_mockCursor.Object);

            // Act
            var result = await _repository.GetByClienteIdAsync("cliente123");

            // Assert
            Assert.Single(result);
            Assert.Equal("cliente123", result.First().ClienteId);
        }

        [Fact]
        public async Task GetByEstadoAsync_RetornaPedidosEnEstado()
        {
            // Arrange
            var pedido = CrearPedidoConId("p-2");

            _mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(true).ReturnsAsync(false);
            _mockCursor.SetupGet(c => c.Current).Returns(new List<Pedido> { pedido });

            _mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Pedido>>(),
                It.IsAny<FindOptions<Pedido, Pedido>>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(_mockCursor.Object);

            // Act
            var result = await _repository.GetByEstadoAsync(pedido.Estado);

            // Assert
            Assert.Single(result);
            Assert.Equal(EstadoPedido.CONFIRMADO, result.First().Estado);
        }

        [Fact]
        public async Task AddAsync_Llama_InsertOneAsync()
        {
            // Arrange
            var direccion = new DireccionEntrega("Calle 1", "123", "Medellin", "050001", "Casa");
            var pedido = new Pedido("cliente321", direccion);

            // Act
            await _repository.AddAsync(pedido);

            // Assert
            _mockCollection.Verify(c =>
                c.InsertOneAsync(pedido, null, default), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_Llama_ReplaceOneAsync()
        {
            // Arrange
            var pedido = CrearPedidoConId("p-10");

            // Act
            await _repository.UpdateAsync(pedido);

            // Assert
            _mockCollection.Verify(c =>
                c.ReplaceOneAsync(It.IsAny<FilterDefinition<Pedido>>(), pedido, It.IsAny<ReplaceOptions>(), default),
                Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_Llama_DeleteOneAsync()
        {
            // Arrange
            var id = "p-15";

            // Act
            await _repository.DeleteAsync(id);

            // Assert
            _mockCollection.Verify(c =>
                c.DeleteOneAsync(It.IsAny<FilterDefinition<Pedido>>(), default),
                Times.Once);
        }
    }
}
