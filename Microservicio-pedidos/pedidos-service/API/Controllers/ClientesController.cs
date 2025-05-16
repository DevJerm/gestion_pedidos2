using Microsoft.AspNetCore.Mvc;
using pedidos_service.Application.DTOs;
using pedidos_service.Domain.Entities;
using pedidos_service.Domain.Repositories;
using pedidos_service.Domain.ValueObjects;

namespace pedidos_service.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(
            IClienteRepository clienteRepository,
            ILogger<ClientesController> logger)
        {
            _clienteRepository = clienteRepository;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ClienteDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CrearCliente([FromBody] CrearClienteDTO crearClienteDTO)
        {
            try
            {
                var direccion = new DireccionEntrega(
                    crearClienteDTO.DireccionEntrega.Calle,
                    crearClienteDTO.DireccionEntrega.Numero,
                    crearClienteDTO.DireccionEntrega.Ciudad,
                    crearClienteDTO.DireccionEntrega.CodigoPostal,
                    crearClienteDTO.DireccionEntrega.Referencia);

                var cliente = new Cliente(
                    crearClienteDTO.Nombre,
                    crearClienteDTO.Email,
                    crearClienteDTO.Telefono,
                    direccion);

                await _clienteRepository.AddAsync(cliente);

                var clienteDTO = new ClienteDTO
                {
                    Id = cliente.Id,
                    Nombre = cliente.Nombre,
                    Email = cliente.Email,
                    Telefono = cliente.Telefono,
                    DireccionPredeterminada = new DireccionEntregaDTO
                    {
                        Calle = cliente.DireccionPredeterminada.Calle,
                        Numero = cliente.DireccionPredeterminada.Numero,
                        Ciudad = cliente.DireccionPredeterminada.Ciudad,
                        CodigoPostal = cliente.DireccionPredeterminada.CodigoPostal,
                        Referencia = cliente.DireccionPredeterminada.Referencia
                    }
                };

                return CreatedAtAction(nameof(ObtenerClientePorId), new { id = cliente.Id }, clienteDTO);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Datos inválidos para crear cliente");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear cliente");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al procesar la solicitud");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClienteDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObtenerClientePorId(string id)
        {
            try
            {
                var cliente = await _clienteRepository.GetByIdAsync(id);
                if (cliente == null)
                    return NotFound($"Cliente con ID {id} no encontrado");

                var clienteDTO = new ClienteDTO
                {
                    Id = cliente.Id,
                    Nombre = cliente.Nombre,
                    Email = cliente.Email,
                    Telefono = cliente.Telefono,
                    DireccionPredeterminada = new DireccionEntregaDTO
                    {
                        Calle = cliente.DireccionPredeterminada.Calle,
                        Numero = cliente.DireccionPredeterminada.Numero,
                        Ciudad = cliente.DireccionPredeterminada.Ciudad,
                        CodigoPostal = cliente.DireccionPredeterminada.CodigoPostal,
                        Referencia = cliente.DireccionPredeterminada.Referencia
                    }
                };

                return Ok(clienteDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cliente");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al procesar la solicitud");
            }
        }
    }
}
