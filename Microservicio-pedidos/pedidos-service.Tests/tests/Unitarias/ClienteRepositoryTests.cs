using MongoDB.Driver;
using Moq;
using pedidos_service.Domain.Entities;
using pedidos_service.Domain.ValueObjects;
using pedidos_service.Infraestructure.Persistence.MongoDB;
using pedidos_service.Infraestructure.Persistence.Repositories;

namespace pedidos_service.tests.Unitarias.Repositories
{
    public class ClienteRepositoryTests
    {
        private readonly Mock<IMongoCollection<Cliente>> _mockCollection;
        private readonly Mock<IAsyncCursor<Cliente>> _mockCursor;
        private readonly ClienteRepository _repository;

        public ClienteRepositoryTests()
        {
            _mockCollection = new Mock<IMongoCollection<Cliente>>();
            _mockCursor = new Mock<IAsyncCursor<Cliente>>();

            var mockContext = new Mock<IMongoDBContext>();
            mockContext.Setup(c => c.Clientes).Returns(_mockCollection.Object);

            _repository = new ClienteRepository(mockContext.Object);
        }

        private Cliente CrearClienteConId(string id)
        {
            var direccion = new DireccionEntrega("Calle33", "47-35", "Medellin", "050000", "parquesito");
            var cliente = new Cliente("Cliente1", "c1@mail.com", "123", direccion);

            typeof(Cliente).GetProperty("Id")?.SetValue(cliente, id);

            return cliente;
        }

        [Fact]
        public async Task GetByIdAsync_RetornaCliente_CuandoExiste()
        {
            // Arrange
            var id = "da5004fb-191c-4f75-88f2-e9ec77afa6f0"; // seteo el id directamente para evitar error en la prueba
            var clienteEsperado = CrearClienteConId(id);

            _mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(true).ReturnsAsync(false);
            _mockCursor.SetupGet(c => c.Current).Returns(new List<Cliente> { clienteEsperado });

            _mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Cliente>>(),
                It.IsAny<FindOptions<Cliente, Cliente>>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(_mockCursor.Object);

            // Act
            var cliente = await _repository.GetByIdAsync(id);

            // Assert
            Assert.NotNull(cliente);
            Assert.Equal("Cliente1", cliente.Nombre);
        }

        [Fact]
        public async Task GetByEmailAsync_RetornaCliente_CuandoExiste()
        {
            // Arrange
            var clienteEsperado = CrearClienteConId("some-id");

            _mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(true).ReturnsAsync(false);
            _mockCursor.SetupGet(c => c.Current).Returns(new List<Cliente> { clienteEsperado });

            _mockCollection.Setup(c => c.FindAsync(
                It.Is<FilterDefinition<Cliente>>(f => true),
                It.IsAny<FindOptions<Cliente, Cliente>>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(_mockCursor.Object);

            // Act
            var cliente = await _repository.GetByEmailAsync("c1@mail.com");

            // Assert
            Assert.NotNull(cliente);
            Assert.Equal("Cliente1", cliente.Nombre);
        }

        [Fact]
        public async Task GetAllAsync_RetornaTodosLosClientes()
        {
            // Arrange
            var direccion = new DireccionEntrega("Calle33", "47-35", "Medellin", "050000", "parquesito");
            var listaClientes = new List<Cliente>
    {
        new Cliente("Cliente1", "c1@mail.com", "123", direccion),
        new Cliente("Cliente2", "c2@mail.com", "456", direccion)
    };

            _mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(true).ReturnsAsync(false);
            _mockCursor.SetupGet(c => c.Current).Returns(listaClientes);

            _mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Cliente>>(),
                It.IsAny<FindOptions<Cliente, Cliente>>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(_mockCursor.Object);

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }
    }
}
