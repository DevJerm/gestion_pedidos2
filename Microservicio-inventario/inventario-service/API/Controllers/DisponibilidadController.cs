using inventario_service.Application.DTOs;
using inventario_service.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace inventario_service.Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DisponibilidadController : ControllerBase
    {
        private readonly IValidacionDisponibilidadService _validacionService;
        private readonly IInventarioService _inventarioService;

        public DisponibilidadController(
            IValidacionDisponibilidadService validacionService,
            IInventarioService inventarioService)
        {
            _validacionService = validacionService;
            _inventarioService = inventarioService;
        }

        [HttpPost("validar")]
        public async Task<ActionResult<bool>> ValidarDisponibilidad(ValidarDisponibilidadRequestDTO request)
        {
            var ingredientesRequeridos = request.Ingredientes.Select(i => new IngredienteRequerido
            {
                IngredienteId = i.IngredienteId,
                CantidadRequerida = i.CantidadRequerida
            }).ToList();

            var disponible = await _validacionService.ValidarDisponibilidadIngredientes(ingredientesRequeridos);

            return Ok(disponible);
        }

        [HttpPost("validar-por-nombre")]
        public async Task<ActionResult<bool>> ValidarDisponibilidadPorNombre(ValidarDisponibilidadPorNombreRequestDTO request)
        {
            var ingredientesRequeridos = request.Ingredientes.Select(i => new IngredienteRequeridoPorNombre
            {
                Nombre = i.Nombre,
                CantidadRequerida = i.CantidadRequerida
            }).ToList();

            var disponible = await _validacionService.ValidarDisponibilidadIngredientesPorNombre(ingredientesRequeridos);

            return Ok(disponible);
        }

        [HttpPost("actualizar")]
        public async Task<ActionResult> ActualizarInventario(ValidarDisponibilidadRequestDTO request)
        {
            var ingredientesRequeridos = request.Ingredientes.Select(i => new IngredienteRequerido
            {
                IngredienteId = i.IngredienteId,
                CantidadRequerida = i.CantidadRequerida
            }).ToList();

            // Primero validamos disponibilidad
            var disponible = await _validacionService.ValidarDisponibilidadIngredientes(ingredientesRequeridos);

            if (!disponible)
            {
                return BadRequest("No hay suficientes ingredientes disponibles para actualizar el inventario.");
            }

            // Si hay disponibilidad, actualizamos el inventario
            await _inventarioService.ActualizarInventario(ingredientesRequeridos);

            return Ok();
        }
    }
}
