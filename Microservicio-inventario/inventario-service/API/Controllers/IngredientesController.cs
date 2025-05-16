using inventario_service.Application.DTOs;
using inventario_service.Domain.Entities;
using inventario_service.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace inventario_service.Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientesController : ControllerBase
    {
        private readonly IInventarioService _inventarioService;

        public IngredientesController(IInventarioService inventarioService)
        {
            _inventarioService = inventarioService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<IngredienteDTO>>> GetAll()
        {
            var ingredientes = await _inventarioService.GetAllIngredientesAsync();
            var ingredientesDTO = new List<IngredienteDTO>();

            foreach (var ingrediente in ingredientes)
            {
                ingredientesDTO.Add(new IngredienteDTO
                {
                    Id = ingrediente.Id,
                    Nombre = ingrediente.Nombre,
                    Cantidad = ingrediente.Cantidad,
                    UnidadMedida = ingrediente.UnidadMedida,
                    Disponible = ingrediente.Disponible
                });
            }

            return Ok(ingredientesDTO);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IngredienteDTO>> GetById(string id)
        {
            var ingrediente = await _inventarioService.GetIngredienteByIdAsync(id);

            if (ingrediente == null)
            {
                return NotFound();
            }

            var ingredienteDTO = new IngredienteDTO
            {
                Id = ingrediente.Id,
                Nombre = ingrediente.Nombre,
                Cantidad = ingrediente.Cantidad,
                UnidadMedida = ingrediente.UnidadMedida,
                Disponible = ingrediente.Disponible
            };

            return Ok(ingredienteDTO);
        }

        [HttpPost]
        public async Task<ActionResult> Create(IngredienteDTO ingredienteDTO)
        {
            var ingrediente = new Ingrediente
            {
                Nombre = ingredienteDTO.Nombre,
                Cantidad = ingredienteDTO.Cantidad,
                UnidadMedida = ingredienteDTO.UnidadMedida,
                Disponible = ingredienteDTO.Disponible
            };

            await _inventarioService.CreateIngredienteAsync(ingrediente);

            return CreatedAtAction(nameof(GetById), new { id = ingrediente.Id }, ingredienteDTO);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, IngredienteDTO ingredienteDTO)
        {
            var ingrediente = await _inventarioService.GetIngredienteByIdAsync(id);

            if (ingrediente == null)
            {
                return NotFound();
            }

            ingrediente.Nombre = ingredienteDTO.Nombre;
            ingrediente.Cantidad = ingredienteDTO.Cantidad;
            ingrediente.UnidadMedida = ingredienteDTO.UnidadMedida;
            ingrediente.Disponible = ingredienteDTO.Disponible;

            await _inventarioService.UpdateIngredienteAsync(ingrediente);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var ingrediente = await _inventarioService.GetIngredienteByIdAsync(id);

            if (ingrediente == null)
            {
                return NotFound();
            }

            await _inventarioService.DeleteIngredienteAsync(id);

            return NoContent();
        }
    }
}
