using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using pedidos_service.API.Controllers;
using pedidos_service.Application.DTOs;
using pedidos_service.Application.Interfaces;
using Xunit;

public class PedidosControllerTests
{
    private readonly Mock<IPedidoService> _pedidoServiceMock;
    private readonly Mock<ILogger<PedidosController>> _loggerMock;
    private readonly PedidosController _controller;

    public PedidosControllerTests()
    {
        _pedidoServiceMock = new Mock<IPedidoService>();
        _loggerMock = new Mock<ILogger<PedidosController>>();
        _controller = new PedidosController(_pedidoServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CrearPedido_RetornaOK()
    {
        var pedidoDTO = new PedidoDTO { Id = "1", Estado = "Pendiente" };
        var crearPedidoDTO = new CrearPedidoDTO();
        _pedidoServiceMock.Setup(s => s.CrearPedidoAsync(crearPedidoDTO))
                          .ReturnsAsync(pedidoDTO);

        var result = await _controller.CrearPedido(crearPedidoDTO);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
    }



    [Fact]
    public async Task ObtenerPedidoPorId_RetorrnaOk()
    {
        var pedidoDTO = new PedidoDTO { Id = "1", Estado = "Pendiente" };
        _pedidoServiceMock.Setup(s => s.GetPedidoByIdAsync("1"))
                          .ReturnsAsync(pedidoDTO);

        var result = await _controller.ObtenerPedidoPorId("1");

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task ObtenerPedidoPorId_RetornaNoEncontrado_PedidoNoExiste()
    {
        _pedidoServiceMock.Setup(s => s.GetPedidoByIdAsync("1"))
                          .ThrowsAsync(new KeyNotFoundException("Pedido no encontrado"));

        var result = await _controller.ObtenerPedidoPorId("1");

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task ActualizarEstadoPedido_RetornaOK()
    {
        var pedidoDTO = new PedidoDTO { Id = "1", Estado = "Enviado" };
        _pedidoServiceMock.Setup(s => s.ActualizarEstadoPedidoAsync("1", "Enviado"))
                          .ReturnsAsync(pedidoDTO);

        var result = await _controller.ActualizarEstadoPedido("1", new CambioEstadoDTO { NuevoEstado = "Enviado" });

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }
    [Fact]
    public async Task ObtenerPedidosPorCliente_RetornaOk()
    {
        var clienteId = "cliente123";
        var pedidos = new List<PedidoDTO>
    {
        new PedidoDTO { Id = "1", Estado = "Pendiente" },
        new PedidoDTO { Id = "2", Estado = "Enviado" }
    };

        _pedidoServiceMock.Setup(s => s.GetPedidosByClienteIdAsync(clienteId))
                          .ReturnsAsync(pedidos);

        var result = await _controller.ObtenerPedidosPorCliente(clienteId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(pedidos, okResult.Value);
    }
    [Fact]
    public async Task ObtenerPedidosPorEstado_RetornaOk()
    {
        var estado = "Pendiente";
        var pedidos = new List<PedidoDTO>
    {
        new PedidoDTO { Id = "1", Estado = "Pendiente" },
        new PedidoDTO { Id = "2", Estado = "Pendiente" }
    };

        _pedidoServiceMock.Setup(s => s.GetPedidosByEstadoAsync(estado))
                          .ReturnsAsync(pedidos);

        var result = await _controller.ObtenerPedidosPorEstado(estado);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(pedidos, okResult.Value);
    }
    [Fact]
    public async Task ObtenerPedidosPorEstado_EstadoInvalido_RetornaBadRequest()
    {
        _pedidoServiceMock.Setup(s => s.GetPedidosByEstadoAsync("invalido"))
                          .ThrowsAsync(new ArgumentException("Estado inválido"));

        var result = await _controller.ObtenerPedidosPorEstado("invalido");

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
    }
    [Fact]
    public async Task CrearPedido_DatosInvalidos_RetornaBadRequest()
    {
        var crearPedidoDTO = new CrearPedidoDTO();

        _pedidoServiceMock.Setup(s => s.CrearPedidoAsync(crearPedidoDTO))
                          .ThrowsAsync(new ArgumentException("Datos inválidos"));

        var result = await _controller.CrearPedido(crearPedidoDTO);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
    }
    [Fact]
    public async Task ActualizarEstadoPedido_PedidoNoExiste_RetornaNotFound()
    {
        _pedidoServiceMock.Setup(s => s.ActualizarEstadoPedidoAsync("123", "Cancelado"))
                          .ThrowsAsync(new KeyNotFoundException("Pedido no encontrado"));

        var result = await _controller.ActualizarEstadoPedido("123", new CambioEstadoDTO { NuevoEstado = "Cancelado" });

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFound.StatusCode);
    }
    [Fact]
    public async Task ActualizarEstadoPedido_EstadoInvalido_RetornaBadRequest()
    {
        _pedidoServiceMock.Setup(s => s.ActualizarEstadoPedidoAsync("1", "Inexistente"))
                          .ThrowsAsync(new ArgumentException("Estado inválido"));

        var result = await _controller.ActualizarEstadoPedido("1", new CambioEstadoDTO { NuevoEstado = "Inexistente" });

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
    }
    [Fact]
    public async Task CrearPedido_ErrorInterno_RetornaStatus500()
    {
        var crearPedidoDTO = new CrearPedidoDTO();

        _pedidoServiceMock.Setup(s => s.CrearPedidoAsync(crearPedidoDTO))
                          .ThrowsAsync(new Exception("Error inesperado"));

        var result = await _controller.CrearPedido(crearPedidoDTO);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
    }
    [Fact]
    public async Task ObtenerPedidoPorId_ErrorInterno_RetornaStatus500()
    {
        _pedidoServiceMock.Setup(s => s.GetPedidoByIdAsync("1"))
                          .ThrowsAsync(new Exception("Error inesperado"));

        var result = await _controller.ObtenerPedidoPorId("1");

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
    }
    [Fact]
    public async Task ObtenerPedidosPorCliente_ErrorInterno_RetornaStatus500()
    {
        var clienteId = "cliente123";
        _pedidoServiceMock.Setup(s => s.GetPedidosByClienteIdAsync(clienteId))
                          .ThrowsAsync(new Exception("Fallo al obtener pedidos"));

        var result = await _controller.ObtenerPedidosPorCliente(clienteId);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
    }
    [Fact]
    public async Task ObtenerPedidosPorEstado_ErrorInterno_RetornaStatus500()
    {
        var estado = "Pendiente";
        _pedidoServiceMock.Setup(s => s.GetPedidosByEstadoAsync(estado))
                          .ThrowsAsync(new Exception("Error inesperado"));

        var result = await _controller.ObtenerPedidosPorEstado(estado);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
    }

}
