using inventario_service.Domain.Entities;
using inventario_service.Infraestructure.Persistence.MongoDB;
using inventario_service.Infrastructure.Persistence.MongoDB;
using MongoDB.Driver;
using Moq;

namespace inventario_service.Test.Tests.Units
{
    public class RecetaRepositoryTests
    {
        private readonly Mock<IMongoDBContext> _mockContext;
        private readonly Mock<IMongoCollection<Receta>> _mockCollection;
        private readonly RecetaRepository _repository;

        public RecetaRepositoryTests()
        {
            _mockContext = new Mock<IMongoDBContext>();
            _mockCollection = new Mock<IMongoCollection<Receta>>();
            _mockContext.Setup(c => c.Recetas).Returns(_mockCollection.Object);
            _repository = new RecetaRepository(_mockContext.Object);
        }

        [Fact]
        public async Task CreateAsync_CallsInsertOne()
        {
            var receta = new Receta { Id = "3", Nombre = "Sopa" };

            await _repository.CreateAsync(receta);

            _mockCollection.Verify(c => c.InsertOneAsync(receta, null, default), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_CallsReplaceOne()
        {
            var receta = new Receta { Id = "1", Nombre = "Arroz con pollo" };

            await _repository.UpdateAsync(receta);

            _mockCollection.Verify(c => c.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Receta>>(),
                receta,
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_CallsDeleteOne()
        {
            await _repository.DeleteAsync("1");

            _mockCollection.Verify(c => c.DeleteOneAsync(
                It.IsAny<FilterDefinition<Receta>>(),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }
    }
}
