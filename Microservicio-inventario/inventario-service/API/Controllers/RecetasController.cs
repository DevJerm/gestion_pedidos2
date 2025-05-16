using System.Collections.Generic;
using System.Threading.Tasks;
using inventario_service.Application.DTOs;
using inventario_service.Application.Services;
using inventario_service.Domain.Entities;
using inventario_service.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace inventario_service.Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecetasController : ControllerBase
    {
        private readonly IRecetaService _recetaService;

        public RecetasController(IRecetaService recetaService)
        {
            _recetaService = recetaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecetaDTO>>> GetAll()
        {
            var recetas = await _recetaService.GetAllRecetasAsync();
            var recetasDTO = new List<RecetaDTO>();

            foreach (var receta in recetas)
            {
                var recetaDTO = new RecetaDTO
                {
                    Id = receta.Id,
                    Nombre = receta.Nombre,
                    Ingredientes = receta.Ingredientes.ConvertAll(i => new IngredienteRecetaDTO
                    {
                        IngredienteId = i.IngredienteId,
                        Cantidad = i.Cantidad
                    })
                };

                recetasDTO.Add(recetaDTO);
            }

            return Ok(recetasDTO);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RecetaDTO>> GetById(string id)
        {
            var receta = await _recetaService.GetRecetaByIdAsync(id);

            if (receta == null)
            {
                return NotFound();
            }

            var recetaDTO = new RecetaDTO
            {
                Id = receta.Id,
                Nombre = receta.Nombre,
                Ingredientes = receta.Ingredientes.ConvertAll(i => new IngredienteRecetaDTO
                {
                    IngredienteId = i.IngredienteId,
                    Cantidad = i.Cantidad
                })
            };

            return Ok(recetaDTO);
        }

        [HttpPost]
        public async Task<ActionResult> Create(RecetaDTO recetaDTO)
        {
            var receta = new Receta
            {
                Nombre = recetaDTO.Nombre,
                Ingredientes = recetaDTO.Ingredientes.ConvertAll(i => new IngredienteReceta
                {
                    IngredienteId = i.IngredienteId,
                    Cantidad = i.Cantidad
                })
            };

            await _recetaService.CreateRecetaAsync(receta);

            return CreatedAtAction(nameof(GetById), new { id = receta.Id }, recetaDTO);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, RecetaDTO recetaDTO)
        {
            var receta = await _recetaService.GetRecetaByIdAsync(id);

            if (receta == null)
            {
                return NotFound();
            }

            receta.Nombre = recetaDTO.Nombre;
            receta.Ingredientes = recetaDTO.Ingredientes.ConvertAll(i => new IngredienteReceta
            {
                IngredienteId = i.IngredienteId,
                Cantidad = i.Cantidad
            });

            await _recetaService.UpdateRecetaAsync(receta);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var receta = await _recetaService.GetRecetaByIdAsync(id);

            if (receta == null)
            {
                return NotFound();
            }

            await _recetaService.DeleteRecetaAsync(id);

            return NoContent();
        }
    }
}
