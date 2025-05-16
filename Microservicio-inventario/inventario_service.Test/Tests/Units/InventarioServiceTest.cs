using inventario_service.Application.Services;
using inventario_service.Domain.Entities;
using inventario_service.Domain.Repositories;
using inventario_service.Domain.Services;
using Moq;
using Xunit;

namespace inventario_service.Tests.Units
{
    public class InventarioServiceTests
    {
        private readonly Mock<IIngredienteRepository> _mockRepository;
        private readonly InventarioService _service;

        public InventarioServiceTests()
        {
            _mockRepository = new Mock<IIngredienteRepository>();
            _service = new InventarioService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAllIngredientesAsync_DebeRetornarTodosLosIngredientes()
        {
            // Arrange
            var ingredientes = new List<Ingrediente>
            {
                new Ingrediente { Id = "1", Nombre = "Tomate", Cantidad = 10, UnidadMedida = "kg", Disponible = true },
                new Ingrediente { Id = "2", Nombre = "Cebolla", Cantidad = 5, UnidadMedida = "kg", Disponible = true }
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(ingredientes);

            // Act
            var resultado = await _service.GetAllIngredientesAsync();

            // Assert
            Assert.Equal(2, ((List<Ingrediente>)resultado).Count);
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetIngredienteByIdAsync_ConIdExistente_DebeRetornarIngrediente()
        {
            // Arrange
            var ingrediente = new Ingrediente
            {
                Id = "1",
                Nombre = "Tomate",
                Cantidad = 10,
                UnidadMedida = "kg",
                Disponible = true
            };

            _mockRepository.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(ingrediente);

            // Act
            var resultado = await _service.GetIngredienteByIdAsync("1");

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Tomate", resultado.Nombre);
            _mockRepository.Verify(r => r.GetByIdAsync("1"), Times.Once);
        }

        [Fact]
        public async Task CreateIngredienteAsync_DebeCrearIngrediente()
        {
            // Arrange
            var ingrediente = new Ingrediente
            {
                Nombre = "Tomate",
                Cantidad = 10,
                UnidadMedida = "kg",
                Disponible = true
            };

            _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Ingrediente>())).Returns(Task.CompletedTask);

            // Act
            await _service.CreateIngredienteAsync(ingrediente);

            // Assert
            _mockRepository.Verify(r => r.CreateAsync(It.Is<Ingrediente>(i => i.Nombre == "Tomate")), Times.Once);
        }

        [Fact]
        public async Task UpdateIngredienteAsync_DebeActualizarIngrediente()
        {
            // Arrange
            var ingrediente = new Ingrediente
            {
                Id = "1",
                Nombre = "Tomate",
                Cantidad = 15,
                UnidadMedida = "kg",
                Disponible = true
            };

            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Ingrediente>())).Returns(Task.CompletedTask);

            // Act
            await _service.UpdateIngredienteAsync(ingrediente);

            // Assert
            _mockRepository.Verify(r => r.UpdateAsync(It.Is<Ingrediente>(i =>
                i.Id == "1" && i.Cantidad == 15)), Times.Once);
        }

        [Fact]
        public async Task DeleteIngredienteAsync_DebeEliminarIngrediente()
        {
            // Arrange
            _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            await _service.DeleteIngredienteAsync("1");

            // Assert
            _mockRepository.Verify(r => r.DeleteAsync("1"), Times.Once);
        }

        [Fact]
        public async Task ActualizarInventario_DebeActualizarCantidadesDeIngredientes()
        {
            // Arrange
            var ingredientesRequeridos = new List<IngredienteRequerido>
            {
                new IngredienteRequerido { IngredienteId = "1", CantidadRequerida = 5 },
                new IngredienteRequerido { IngredienteId = "2", CantidadRequerida = 3 }
            };

            var ingrediente1 = new Ingrediente { Id = "1", Nombre = "Tomate", Cantidad = 10, UnidadMedida = "kg", Disponible = true };
            var ingrediente2 = new Ingrediente { Id = "2", Nombre = "Cebolla", Cantidad = 5, UnidadMedida = "kg", Disponible = true };

            _mockRepository.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(ingrediente1);
            _mockRepository.Setup(r => r.GetByIdAsync("2")).ReturnsAsync(ingrediente2);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Ingrediente>())).Returns(Task.CompletedTask);

            // Act
            await _service.ActualizarInventario(ingredientesRequeridos);

            // Assert
            Assert.Equal(5, ingrediente1.Cantidad); // 10 - 5 = 5
            Assert.Equal(2, ingrediente2.Cantidad); // 5 - 3 = 2
            _mockRepository.Verify(r => r.UpdateAsync(It.Is<Ingrediente>(i => i.Id == "1" && i.Cantidad == 5)), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(It.Is<Ingrediente>(i => i.Id == "2" && i.Cantidad == 2)), Times.Once);
        }
    }
}