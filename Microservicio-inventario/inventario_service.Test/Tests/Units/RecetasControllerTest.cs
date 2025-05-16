using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using inventario_service.Application.Controllers;
using inventario_service.Application.DTOs;
using inventario_service.Domain.Entities;
using inventario_service.Domain.Services;

public class RecetasControllerTests
{
    private readonly Mock<IRecetaService> _recetaServiceMock;
    private readonly RecetasController _controller;

    public RecetasControllerTests()
    {
        _recetaServiceMock = new Mock<IRecetaService>();
        _controller = new RecetasController(_recetaServiceMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_ListaTodasRecetas()
    {
        // Arrange
        var recetas = new List<Receta>
        {
            new Receta { Id = "1", Nombre = "Receta 1", Ingredientes = new List<IngredienteReceta>() },
            new Receta { Id = "2", Nombre = "Receta 2", Ingredientes = new List<IngredienteReceta>() }
        };
        _recetaServiceMock.Setup(s => s.GetAllRecetasAsync()).ReturnsAsync(recetas);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<RecetaDTO>>(okResult.Value);
        Assert.Equal(2, returnValue.Count);
    }

    [Fact]
    public async Task GetById_ExistingId_RetornaReceta()
    {
        // Arrange
        var receta = new Receta { Id = "1", Nombre = "Receta 1", Ingredientes = new List<IngredienteReceta>() };
        _recetaServiceMock.Setup(s => s.GetRecetaByIdAsync("1")).ReturnsAsync(receta);

        // Act
        var result = await _controller.GetById("1");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<RecetaDTO>(okResult.Value);
        Assert.Equal("1", returnValue.Id);
    }

    [Fact]
    public async Task GetById_NonExistingId_NoRetornaReceta()
    {
        // Arrange
        _recetaServiceMock.Setup(s => s.GetRecetaByIdAsync("99")).ReturnsAsync((Receta)null);

        // Act
        var result = await _controller.GetById("99");

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ValidReceta_RetornaRecetaCreada()
    {
        // Arrange
        var recetaDTO = new RecetaDTO
        {
            Id = "1",
            Nombre = "Nueva Receta",
            Ingredientes = new List<IngredienteRecetaDTO>()
        };

        // Act
        var result = await _controller.Create(recetaDTO);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);
    }

    [Fact]
    public async Task Update_ExistingId_RetornaNoEncontrado()
    {
        // Arrange
        var existingReceta = new Receta { Id = "1", Nombre = "Receta Existente", Ingredientes = new List<IngredienteReceta>() };
        var updatedRecetaDTO = new RecetaDTO { Id = "1", Nombre = "Receta Actualizada", Ingredientes = new List<IngredienteRecetaDTO>() };

        _recetaServiceMock.Setup(s => s.GetRecetaByIdAsync("1")).ReturnsAsync(existingReceta);
        _recetaServiceMock.Setup(s => s.UpdateRecetaAsync(It.IsAny<Receta>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update("1", updatedRecetaDTO);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        _recetaServiceMock.Setup(s => s.GetRecetaByIdAsync("99")).ReturnsAsync((Receta)null);
        var recetaDTO = new RecetaDTO { Id = "99", Nombre = "Nueva Receta", Ingredientes = new List<IngredienteRecetaDTO>() };

        // Act
        var result = await _controller.Update("99", recetaDTO);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ExistingId_NoEncontrado()
    {
        // Arrange
        var receta = new Receta { Id = "1", Nombre = "Receta eliminarr JERM", Ingredientes = new List<IngredienteReceta>() };
        _recetaServiceMock.Setup(s => s.GetRecetaByIdAsync("1")).ReturnsAsync(receta);
        _recetaServiceMock.Setup(s => s.DeleteRecetaAsync("1")).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete("1");

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_NonExistingId_RetornaNoEncontrado()
    {
        // Arrange
        _recetaServiceMock.Setup(s => s.GetRecetaByIdAsync("99")).ReturnsAsync((Receta)null);

        // Act
        var result = await _controller.Delete("99");

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_ValidReceta_LlamaAlServicioConDatosCorrectos()
    {
        // Arrange
        var recetaDTO = new RecetaDTO
        {
            Nombre = "Test Receta",
            Ingredientes = new List<IngredienteRecetaDTO>
        {
            new IngredienteRecetaDTO { IngredienteId = "1", Cantidad = 2 }
        }
        };

        Receta recetaCreada = null;
        _recetaServiceMock
            .Setup(s => s.CreateRecetaAsync(It.IsAny<Receta>()))
            .Callback<Receta>(r => recetaCreada = r)
            .Returns(Task.CompletedTask);

        // Act
        await _controller.Create(recetaDTO);

        // Assert
        _recetaServiceMock.Verify(s => s.CreateRecetaAsync(It.IsAny<Receta>()), Times.Once);
        Assert.NotNull(recetaCreada);
        Assert.Equal(recetaDTO.Nombre, recetaCreada.Nombre);
        Assert.Single(recetaCreada.Ingredientes);
        Assert.Equal("1", recetaCreada.Ingredientes[0].IngredienteId);
        Assert.Equal(2, recetaCreada.Ingredientes[0].Cantidad);
    }
    [Fact]
    public async Task Update_Receta_ActualizaCamposCorrectamente()
    {
        // Arrange
        var recetaExistente = new Receta
        {
            Id = "1",
            Nombre = "Viejo Nombre",
            Ingredientes = new List<IngredienteReceta>()
        };

        var recetaDTO = new RecetaDTO
        {
            Id = "1",
            Nombre = "Nuevo Nombre",
            Ingredientes = new List<IngredienteRecetaDTO>
        {
            new IngredienteRecetaDTO { IngredienteId = "2", Cantidad = 5 }
        }
        };

        _recetaServiceMock.Setup(s => s.GetRecetaByIdAsync("1")).ReturnsAsync(recetaExistente);
        _recetaServiceMock.Setup(s => s.UpdateRecetaAsync(It.IsAny<Receta>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update("1", recetaDTO);

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.Equal("Nuevo Nombre", recetaExistente.Nombre);
        Assert.Single(recetaExistente.Ingredientes);
        Assert.Equal("2", recetaExistente.Ingredientes[0].IngredienteId);
        Assert.Equal(5, recetaExistente.Ingredientes[0].Cantidad);
    }


}
