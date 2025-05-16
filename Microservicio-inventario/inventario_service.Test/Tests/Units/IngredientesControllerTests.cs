using Microsoft.AspNetCore.Mvc;
using inventario_service.Application.Controllers;
using inventario_service.Application.DTOs;
using inventario_service.Domain.Entities;
using inventario_service.Domain.Services;
using Moq;
using Xunit;

public class IngredientesControllerTests
{
    private readonly Mock<IInventarioService> _inventarioServiceMock;
    private readonly IngredientesController _controller;

    public IngredientesControllerTests()
    {
        _inventarioServiceMock = new Mock<IInventarioService>();
        _controller = new IngredientesController(_inventarioServiceMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_RetornaListaIngredientes()
    {
        // Arrange
        var ingredientes = new List<Ingrediente>
        {
            new Ingrediente { Id = "1", Nombre = "Harina", Cantidad = 5, UnidadMedida = "kg", Disponible = true },
            new Ingrediente { Id = "2", Nombre = "Azucar", Cantidad = 3, UnidadMedida = "kg", Disponible = false }
        };
        _inventarioServiceMock.Setup(s => s.GetAllIngredientesAsync()).ReturnsAsync(ingredientes);

        // Act
        var resultado = await _controller.GetAll();

        // Assert
        var okResultado = Assert.IsType<OkObjectResult>(resultado.Result);
        var returnValue = Assert.IsType<List<IngredienteDTO>>(okResultado.Value);
        Assert.Equal(2, returnValue.Count);
    }

    [Fact]
    public async Task GetById_ExistingId_RetornaOK()
    {
        // Arrange
        var ingrediente = new Ingrediente { Id = "1", Nombre = "Harina", Cantidad = 5, UnidadMedida = "kg", Disponible = true };
        _inventarioServiceMock.Setup(s => s.GetIngredienteByIdAsync("1")).ReturnsAsync(ingrediente);

        // Act
        var resultado = await _controller.GetById("1");

        // Assert
        var okResultado = Assert.IsType<OkObjectResult>(resultado.Result);
        var returnValue = Assert.IsType<IngredienteDTO>(okResultado.Value);
        Assert.Equal("1", returnValue.Id);
    }

    [Fact]
    public async Task GetById_NonExistingId_RetornaNoEncontrado()
    {
        // Arrange
        _inventarioServiceMock.Setup(s => s.GetIngredienteByIdAsync("99")).ReturnsAsync((Ingrediente)null);

        // Act
        var result = await _controller.GetById("99");

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ValidIngrediente_RetornaOK()
    {
        // Arrange
        var ingredienteDTO = new IngredienteDTO
        {
            Id = "1",
            Nombre = "Sal",
            Cantidad = 2,
            UnidadMedida = "kg",
            Disponible = true
        };

        // Act
        var result = await _controller.Create(ingredienteDTO);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);
    }

    [Fact]
    public async Task Update_ExistingId_RetornaNoEncontrado()
    {
        // Arrange
        var existingIngrediente = new Ingrediente { Id = "1", Nombre = "Harina", Cantidad = 5, UnidadMedida = "kg", Disponible = true };
        var updatedIngredienteDTO = new IngredienteDTO { Id = "1", Nombre = "Harina Integral", Cantidad = 4, UnidadMedida = "kg", Disponible = true };

        _inventarioServiceMock.Setup(s => s.GetIngredienteByIdAsync("1")).ReturnsAsync(existingIngrediente);
        _inventarioServiceMock.Setup(s => s.UpdateIngredienteAsync(It.IsAny<Ingrediente>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update("1", updatedIngredienteDTO);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_NonExistingId_RetornaNoEncontrado()
    {
        // Arrange
        _inventarioServiceMock.Setup(s => s.GetIngredienteByIdAsync("99")).ReturnsAsync((Ingrediente)null);
        var ingredienteDTO = new IngredienteDTO { Id = "99", Nombre = "Leche", Cantidad = 1, UnidadMedida = "L", Disponible = true };

        // Act
        var resultado = await _controller.Update("99", ingredienteDTO);

        // Assert
        Assert.IsType<NotFoundResult>(resultado);
    }

    [Fact]
    public async Task Delete_ExistingId_RetornaNoEncontrado()
    {
        // Arrange
        var ingrediente = new Ingrediente { Id = "1", Nombre = "Mantequilla", Cantidad = 2, UnidadMedida = "kg", Disponible = true };
        _inventarioServiceMock.Setup(s => s.GetIngredienteByIdAsync("1")).ReturnsAsync(ingrediente);
        _inventarioServiceMock.Setup(s => s.DeleteIngredienteAsync("1")).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete("1");

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_NonExistingId_RetornaNoEncontrado()
    {
        // Arrange
        _inventarioServiceMock.Setup(s => s.GetIngredienteByIdAsync("99")).ReturnsAsync((Ingrediente)null);

        // Act
        var resultado = await _controller.Delete("99");

        // Assert
        Assert.IsType<NotFoundResult>(resultado);
    }
}
