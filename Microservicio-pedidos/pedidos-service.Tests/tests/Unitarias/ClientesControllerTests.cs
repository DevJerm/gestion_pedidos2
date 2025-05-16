using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using pedidos_service.API.Controllers;
using pedidos_service.Application.DTOs;
using pedidos_service.Domain.Entities;
using pedidos_service.Domain.Repositories;
using pedidos_service.Domain.ValueObjects;
using Xunit;

public class ClientesControllerTests
{
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly Mock<ILogger<ClientesController>> _loggerMock;
    private readonly ClientesController _controller;

    public ClientesControllerTests()
    {
        _clienteRepositoryMock = new Mock<IClienteRepository>();
        _loggerMock = new Mock<ILogger<ClientesController>>();
        _controller = new ClientesController(_clienteRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ObtenerClientePorId_Retorna200OK_ClienteExiste()
    {
        // Arrange
        var clienteId = "123";
        var cliente = new Cliente("pepito", "pepito@itm.com", "30123456",
            new DireccionEntrega("Cr33", "4735", "medellin", "05000", "buenos"));

        _clienteRepositoryMock.Setup(repo => repo.GetByIdAsync(clienteId))
            .ReturnsAsync(cliente);

        // Act
        var result = await _controller.ObtenerClientePorId(clienteId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task ObtenerClientePorId_Retorna404NotFound_clienteNoExiste()
    {
        // Arrange
        var clienteId = "999";
        _clienteRepositoryMock.Setup(repo => repo.GetByIdAsync(clienteId))
            .ReturnsAsync((Cliente)null);

        // Act
        var result = await _controller.ObtenerClientePorId(clienteId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task CrearCliente_Valido_RetornaCreated()
    {
        // Arrange
        var crearClienteDTO = new CrearClienteDTO
        {
            Nombre = "pepito",
            Email = "pepito@itm.com",
            Telefono = "30123456",
            DireccionEntrega = new DireccionEntregaDTO
            {
                Calle = "Cr33",
                Numero = "4735",
                Ciudad = "medellin",
                CodigoPostal = "05000",
                Referencia = "buenos"
            }
        };

        Cliente clienteGuardado = null;

        _clienteRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Cliente>()))
            .Callback<Cliente>(c => clienteGuardado = c)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.CrearCliente(crearClienteDTO);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);

        var clienteDTO = Assert.IsType<ClienteDTO>(createdResult.Value);
        Assert.Equal(crearClienteDTO.Nombre, clienteDTO.Nombre);
        Assert.Equal(crearClienteDTO.Email, clienteDTO.Email);
    }

    [Fact]
    public async Task CrearCliente_DatosInvalidos_RetornaBadRequest()
    {
        // Arrange
        var crearClienteDTO = new CrearClienteDTO
        {
            Nombre = "pepito malo",
            Email = "pepito@mal.com",
            Telefono = "123456",
            DireccionEntrega = new DireccionEntregaDTO
            {
                Calle = "cr",
                Numero = "22",
                Ciudad = "medellinnn",
                CodigoPostal = "12345",
                Referencia = "nada"
            }
        };

        _clienteRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Cliente>()))
            .ThrowsAsync(new ArgumentException("Datos inválidos"));

        // Act
        var result = await _controller.CrearCliente(crearClienteDTO);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
        Assert.Equal("Datos inválidos", badRequest.Value);
    }

    [Fact]
    public async Task CrearCliente_ErrorInterno_RetornaStatus500()
    {
        // Arrange
        var crearClienteDTO = new CrearClienteDTO
        {
            Nombre = "pepito",
            Email = "pepito@mal.com",
            Telefono = "123456",
            DireccionEntrega = new DireccionEntregaDTO
            {
                Calle = "cr33",
                Numero = "4745",
                Ciudad = "medellinn",
                CodigoPostal = "11121",
                Referencia = "Error"
            }
        };

        _clienteRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Cliente>()))
            .ThrowsAsync(new Exception("Error inesperado"));

        // Act
        var result = await _controller.CrearCliente(crearClienteDTO);

        // Assert
        var errorResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, errorResult.StatusCode);
        Assert.Equal("Error al procesar la solicitud", errorResult.Value);
    }

}
