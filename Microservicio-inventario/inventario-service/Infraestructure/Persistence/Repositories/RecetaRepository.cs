using inventario_service.Domain.Entities;
using inventario_service.Domain.Repositories;
using inventario_service.Infraestructure.Persistence.MongoDB;
using MongoDB.Driver;

namespace inventario_service.Infrastructure.Persistence.MongoDB
{
    public class RecetaRepository : IRecetaRepository
    {
        private readonly IMongoDBContext _context;

        public RecetaRepository(IMongoDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Receta>> GetAllAsync()
        {
            return await _context.Recetas.Find(_ => true).ToListAsync();
        }

        public async Task<Receta> GetByIdAsync(string id)
        {
            return await _context.Recetas.Find(r => r.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Receta> GetByNombreAsync(string nombre)
        {
            return await _context.Recetas.Find(r => r.Nombre == nombre).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Receta receta)
        {
            await _context.Recetas.InsertOneAsync(receta);
        }

        public async Task UpdateAsync(Receta receta)
        {
            await _context.Recetas.ReplaceOneAsync(r => r.Id == receta.Id, receta);
        }

        public async Task DeleteAsync(string id)
        {
            await _context.Recetas.DeleteOneAsync(r => r.Id == id);
        }
    }
}