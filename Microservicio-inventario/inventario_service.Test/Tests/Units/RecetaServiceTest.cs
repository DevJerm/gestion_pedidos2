using inventario_service.Application.Services;
using inventario_service.Domain.Entities;
using inventario_service.Domain.Repositories;
using Moq;
using Xunit;

namespace inventario_service.Tests.Units
{
    public class RecetaServiceTests
    {
        private readonly Mock<IRecetaRepository> _mockRepository;
        private readonly RecetaService _service;

        public RecetaServiceTests()
        {
            _mockRepository = new Mock<IRecetaRepository>();
            _service = new RecetaService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAllRecetasAsync_DebeRetornarTodasLasRecetas()
        {
            // Arrange
            var recetas = new List<Receta>
            {
                new Receta { Id = "1", Nombre = "Pizza", Ingredientes = new List<IngredienteReceta>() },
                new Receta { Id = "2", Nombre = "Hamburguesa", Ingredientes = new List<IngredienteReceta>() }
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(recetas);

            // Act
            var resultado = await _service.GetAllRecetasAsync();

            // Assert
            Assert.Equal(2, ((List<Receta>)resultado).Count);
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetRecetaByIdAsync_ConIdExistente_DebeRetornarReceta()
        {
            // Arrange
            var receta = new Receta
            {
                Id = "1",
                Nombre = "Pizza",
                Ingredientes = new List<IngredienteReceta>
                {
                    new IngredienteReceta { IngredienteId = "1", Cantidad = 2 }
                }
            };

            _mockRepository.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(receta);

            // Act
            var resultado = await _service.GetRecetaByIdAsync("1");

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Pizza", resultado.Nombre);
            Assert.Single(resultado.Ingredientes);
            _mockRepository.Verify(r => r.GetByIdAsync("1"), Times.Once);
        }

        [Fact]
        public async Task GetRecetaByNombreAsync_ConNombreExistente_DebeRetornarReceta()
        {
            // Arrange
            var receta = new Receta
            {
                Id = "1",
                Nombre = "Pizza",
                Ingredientes = new List<IngredienteReceta>()
            };

            _mockRepository.Setup(r => r.GetByNombreAsync("Pizza")).ReturnsAsync(receta);

            // Act
            var resultado = await _service.GetRecetaByNombreAsync("Pizza");

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("1", resultado.Id);
            _mockRepository.Verify(r => r.GetByNombreAsync("Pizza"), Times.Once);
        }

        [Fact]
        public async Task CreateRecetaAsync_DebeCrearReceta()
        {
            // Arrange
            var receta = new Receta
            {
                Nombre = "Pizza",
                Ingredientes = new List<IngredienteReceta>
                {
                    new IngredienteReceta { IngredienteId = "1", Cantidad = 2 }
                }
            };

            _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Receta>())).Returns(Task.CompletedTask);

            // Act
            await _service.CreateRecetaAsync(receta);

            // Assert
            _mockRepository.Verify(r => r.CreateAsync(It.Is<Receta>(r =>
                r.Nombre == "Pizza" && r.Ingredientes.Count == 1)), Times.Once);
        }

        [Fact]
        public async Task UpdateRecetaAsync_DebeActualizarReceta()
        {
            // Arrange
            var receta = new Receta
            {
                Id = "1",
                Nombre = "Pizza Actualizada",
                Ingredientes = new List<IngredienteReceta>
                {
                    new IngredienteReceta { IngredienteId = "1", Cantidad = 3 }
                }
            };

            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Receta>())).Returns(Task.CompletedTask);

            // Act
            await _service.UpdateRecetaAsync(receta);

            // Assert
            _mockRepository.Verify(r => r.UpdateAsync(It.Is<Receta>(r =>
                r.Id == "1" && r.Nombre == "Pizza Actualizada")), Times.Once);
        }

        [Fact]
        public async Task DeleteRecetaAsync_DebeEliminarReceta()
        {
            // Arrange
            _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            await _service.DeleteRecetaAsync("1");

            // Assert
            _mockRepository.Verify(r => r.DeleteAsync("1"), Times.Once);
        }
    }
}