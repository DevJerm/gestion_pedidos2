using inventario_service.Application.Services;
using inventario_service.Domain.Entities;
using inventario_service.Domain.Repositories;
using inventario_service.Domain.Services;
using Moq;
using Xunit;

namespace inventario_service.Tests.Units
{
    public class ValidacionDisponibilidadServiceTests
    {
        private readonly Mock<IIngredienteRepository> _mockRepository;
        private readonly ValidacionDisponibilidadService _service;

        public ValidacionDisponibilidadServiceTests()
        {
            _mockRepository = new Mock<IIngredienteRepository>();
            _service = new ValidacionDisponibilidadService(_mockRepository.Object);
        }

        [Fact]
        public async Task ValidarDisponibilidadIngredientes_ConIngredientesSuficientes_DebeRetornarTrue()
        {
            // Arrange
            var ingredientesRequeridos = new List<IngredienteRequerido>
            {
                new IngredienteRequerido { IngredienteId = "1", CantidadRequerida = 5 },
                new IngredienteRequerido { IngredienteId = "2", CantidadRequerida = 3 }
            };

            var ingredientesDisponibles = new List<Ingrediente>
            {
                new Ingrediente { Id = "1", Nombre = "Tomate", Cantidad = 10, UnidadMedida = "kg", Disponible = true },
                new Ingrediente { Id = "2", Nombre = "Cebolla", Cantidad = 5, UnidadMedida = "kg", Disponible = true }
            };

            _mockRepository.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(ingredientesDisponibles);

            // Act
            var resultado = await _service.ValidarDisponibilidadIngredientes(ingredientesRequeridos);

            // Assert
            Assert.True(resultado);
            _mockRepository.Verify(r => r.GetByIdsAsync(It.Is<IEnumerable<string>>(ids =>
                ids.Count() == 2 && ids.Contains("1") && ids.Contains("2"))), Times.Once);
        }

        [Fact]
        public async Task ValidarDisponibilidadIngredientes_ConIngredientesInsuficientes_DebeRetornarFalse()
        {
            // Arrange
            var ingredientesRequeridos = new List<IngredienteRequerido>
            {
                new IngredienteRequerido { IngredienteId = "1", CantidadRequerida = 15 },
                new IngredienteRequerido { IngredienteId = "2", CantidadRequerida = 3 }
            };

            var ingredientesDisponibles = new List<Ingrediente>
            {
                new Ingrediente { Id = "1", Nombre = "Tomate", Cantidad = 10, UnidadMedida = "kg", Disponible = true },
                new Ingrediente { Id = "2", Nombre = "Cebolla", Cantidad = 5, UnidadMedida = "kg", Disponible = true }
            };

            _mockRepository.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(ingredientesDisponibles);

            // Act
            var resultado = await _service.ValidarDisponibilidadIngredientes(ingredientesRequeridos);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task ValidarDisponibilidadIngredientes_ConIngredienteNoDisponible_DebeRetornarFalse()
        {
            // Arrange
            var ingredientesRequeridos = new List<IngredienteRequerido>
            {
                new IngredienteRequerido { IngredienteId = "1", CantidadRequerida = 5 },
                new IngredienteRequerido { IngredienteId = "2", CantidadRequerida = 3 }
            };

            var ingredientesDisponibles = new List<Ingrediente>
            {
                new Ingrediente { Id = "1", Nombre = "Tomate", Cantidad = 10, UnidadMedida = "kg", Disponible = true },
                new Ingrediente { Id = "2", Nombre = "Cebolla", Cantidad = 5, UnidadMedida = "kg", Disponible = false }
            };

            _mockRepository.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(ingredientesDisponibles);

            // Act
            var resultado = await _service.ValidarDisponibilidadIngredientes(ingredientesRequeridos);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task ValidarDisponibilidadIngredientes_ConIngredienteInexistente_DebeRetornarFalse()
        {
            // Arrange
            var ingredientesRequeridos = new List<IngredienteRequerido>
            {
                new IngredienteRequerido { IngredienteId = "1", CantidadRequerida = 5 },
                new IngredienteRequerido { IngredienteId = "3", CantidadRequerida = 3 }
            };

            var ingredientesDisponibles = new List<Ingrediente>
            {
                new Ingrediente { Id = "1", Nombre = "Tomate", Cantidad = 10, UnidadMedida = "kg", Disponible = true },
                new Ingrediente { Id = "2", Nombre = "Cebolla", Cantidad = 5, UnidadMedida = "kg", Disponible = true }
            };

            _mockRepository.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(ingredientesDisponibles);

            // Act
            var resultado = await _service.ValidarDisponibilidadIngredientes(ingredientesRequeridos);

            // Assert
            Assert.False(resultado);
        }
    }
}