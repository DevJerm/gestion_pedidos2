using inventario_service.Domain.Entities;
using inventario_service.Domain.Repositories;
using MongoDB.Driver;
using inventario_service.Infraestructure.Persistence.MongoDB;

namespace inventario_service.Infraestructure.Persistence.Repositories
{
    public class IngredienteRepository : IIngredienteRepository
    {
        private readonly IMongoDBContext _context;

        public IngredienteRepository(IMongoDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ingrediente>> GetAllAsync()
        {
            return await _context.Ingredientes.Find(_ => true).ToListAsync();
        }

        public async Task<Ingrediente> GetByIdAsync(string id)
        {
            return await _context.Ingredientes.Find(i => i.Id == id).FirstOrDefaultAsync();
        }
        public async Task<List<Ingrediente>> GetByNombresAsync(List<string> nombres)
        {
            return await _context.Ingredientes.Find(i => nombres.Contains(i.Nombre)).ToListAsync();
        }

        public async Task<IEnumerable<Ingrediente>> GetByIdsAsync(IEnumerable<string> ids)
        {
            var filter = Builders<Ingrediente>.Filter.In(i => i.Id, ids);
            return await _context.Ingredientes.Find(filter).ToListAsync();
        }

        public async Task CreateAsync(Ingrediente ingrediente)
        {
            await _context.Ingredientes.InsertOneAsync(ingrediente);
        }

        public async Task UpdateAsync(Ingrediente ingrediente)
        {
            await _context.Ingredientes.ReplaceOneAsync(i => i.Id == ingrediente.Id, ingrediente);
        }

        public async Task DeleteAsync(string id)
        {
            await _context.Ingredientes.DeleteOneAsync(i => i.Id == id);
        }
    }
}