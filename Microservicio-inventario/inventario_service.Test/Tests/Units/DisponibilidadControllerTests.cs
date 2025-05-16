using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using inventario_service.Application.Controllers;
using inventario_service.Application.DTOs;
using inventario_service.Domain.Entities;
using inventario_service.Domain.Services;
using System.Linq;

public class DisponibilidadControllerTests
{
    private readonly Mock<IValidacionDisponibilidadService> _validacionServiceMock;
    private readonly Mock<IInventarioService> _inventarioServiceMock;
    private readonly DisponibilidadController _controller;

    public DisponibilidadControllerTests()
    {
        _validacionServiceMock = new Mock<IValidacionDisponibilidadService>();
        _inventarioServiceMock = new Mock<IInventarioService>();
        _controller = new DisponibilidadController(_validacionServiceMock.Object, _inventarioServiceMock.Object);
    }

    [Fact]
    public async Task ValidarDisponibilidad_Disponible_ReturnsOkTrue()
    {
        // Arrange
        var request = new ValidarDisponibilidadRequestDTO
        {
            Ingredientes = new List<IngredienteRequeridoDTO>
            {
                new IngredienteRequeridoDTO { IngredienteId = "1", CantidadRequerida = 5 }
            }
        };

        _validacionServiceMock
            .Setup(v => v.ValidarDisponibilidadIngredientes(It.IsAny<List<IngredienteRequerido>>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.ValidarDisponibilidad(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.True((bool)okResult.Value);
    }

    [Fact]
    public async Task ValidarDisponibilidadPorNombre_Disponible_ReturnsOkTrue()
    {
        // Arrange
        var request = new ValidarDisponibilidadPorNombreRequestDTO
        {
            Ingredientes = new List<IngredienteRequeridoPorNombre>
            {
                new IngredienteRequeridoPorNombre { Nombre = "Harina", CantidadRequerida = 3 }
            }
        };

        _validacionServiceMock
            .Setup(v => v.ValidarDisponibilidadIngredientesPorNombre(It.IsAny<List<IngredienteRequeridoPorNombre>>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.ValidarDisponibilidadPorNombre(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.True((bool)okResult.Value);
    }

    [Fact]
    public async Task ActualizarInventario_ConDisponibilidad_ReturnsOk()
    {
        // Arrange
        var request = new ValidarDisponibilidadRequestDTO
        {
            Ingredientes = new List<IngredienteRequeridoDTO>
            {
                new IngredienteRequeridoDTO { IngredienteId = "1", CantidadRequerida = 2 }
            }
        };

        _validacionServiceMock
            .Setup(v => v.ValidarDisponibilidadIngredientes(It.IsAny<List<IngredienteRequerido>>()))
            .ReturnsAsync(true);

        _inventarioServiceMock
            .Setup(i => i.ActualizarInventario(It.IsAny<List<IngredienteRequerido>>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ActualizarInventario(request);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task ActualizarInventario_SinDisponibilidad_ReturnsBadRequest()
    {
        // Arrange
        var request = new ValidarDisponibilidadRequestDTO
        {
            Ingredientes = new List<IngredienteRequeridoDTO>
            {
                new IngredienteRequeridoDTO { IngredienteId = "2", CantidadRequerida = 10 }
            }
        };

        _validacionServiceMock
            .Setup(v => v.ValidarDisponibilidadIngredientes(It.IsAny<List<IngredienteRequerido>>()))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.ActualizarInventario(request);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("No hay suficientes ingredientes disponibles para actualizar el inventario.", badRequest.Value);
    }
}
