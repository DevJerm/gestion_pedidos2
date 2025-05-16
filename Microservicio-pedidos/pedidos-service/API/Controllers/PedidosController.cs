using Microsoft.AspNetCore.Mvc;
using pedidos_service.Application.DTOs;
using pedidos_service.Application.Interfaces;

namespace pedidos_service.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;
        private readonly ILogger<PedidosController> _logger;

        public PedidosController(
            IPedidoService pedidoService,
            ILogger<PedidosController> logger)
        {
            _pedidoService = pedidoService;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(PedidoDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CrearPedido([FromBody] CrearPedidoDTO crearPedidoDTO)
        {
            try
            {
                var pedido = await _pedidoService.CrearPedidoAsync(crearPedidoDTO);
                return CreatedAtAction(nameof(ObtenerPedidoPorId), new { id = pedido.Id }, pedido);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Solicitud inválida para crear pedido");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear pedido");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al procesar la solicitud");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PedidoDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObtenerPedidoPorId(string id)
        {
            try
            {
                var pedido = await _pedidoService.GetPedidoByIdAsync(id);
                return Ok(pedido);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Pedido no encontrado");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pedido");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al procesar la solicitud");
            }
        }
        [HttpGet("cliente/{clienteId}")]
        [ProducesResponseType(typeof(IEnumerable<PedidoDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObtenerPedidosPorCliente(string clienteId)
        {
            try
            {
                var pedidos = await _pedidoService.GetPedidosByClienteIdAsync(clienteId);
                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pedidos por cliente");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al procesar la solicitud");
            }
        }

        [HttpGet("estado/{estado}")]
        [ProducesResponseType(typeof(IEnumerable<PedidoDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObtenerPedidosPorEstado(string estado)
        {
            try
            {
                var pedidos = await _pedidoService.GetPedidosByEstadoAsync(estado);
                return Ok(pedidos);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Estado de pedido inválido");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pedidos por estado");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al procesar la solicitud");
            }
        }

        [HttpPut("{id}/estado")]
        [ProducesResponseType(typeof(PedidoDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ActualizarEstadoPedido(string id, [FromBody] CambioEstadoDTO cambioEstadoDTO)
        {
            try
            {
                var pedido = await _pedidoService.ActualizarEstadoPedidoAsync(id, cambioEstadoDTO.NuevoEstado);
                return Ok(pedido);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Pedido no encontrado");
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Estado de pedido inválido o transición no permitida");
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Operación no válida en el pedido");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar estado del pedido");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al procesar la solicitud");
            }
        }
    }
}
