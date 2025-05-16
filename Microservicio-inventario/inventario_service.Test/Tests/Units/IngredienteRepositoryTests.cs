using System.Collections.Generic;
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using inventario_service.Domain.Entities;
using inventario_service.Infraestructure.Persistence.MongoDB;
using inventario_service.Infraestructure.Persistence.Repositories;
using MongoDB.Driver;
using Moq;
using Xunit;

public class IngredienteRepositoryTests
{
    private readonly Mock<IMongoDBContext> _mockContext;
    private readonly Mock<IMongoCollection<Ingrediente>> _mockCollection;
    private readonly IngredienteRepository _repository;

    public IngredienteRepositoryTests()
    {
        _mockContext = new Mock<IMongoDBContext>();
        _mockCollection = new Mock<IMongoCollection<Ingrediente>>();
        _mockContext.Setup(c => c.Ingredientes).Returns(_mockCollection.Object);
        _repository = new IngredienteRepository(_mockContext.Object);
    }

    [Fact]
    public async Task CreateAsync_CallsInsertOne()
    {

        var ingrediente = new Ingrediente { Id = "1", Nombre = "Tomate", Cantidad = 2.0, UnidadMedida = "kg", Disponible = true };
        await _repository.CreateAsync(ingrediente);
        _mockCollection.Verify(c => c.InsertOneAsync(ingrediente, null, default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_CallsReplaceOne()
    {
        var ingrediente = new Ingrediente { Id = "1", Nombre = "Tomate", Cantidad = 2.0, UnidadMedida = "kg", Disponible = true };

        await _repository.UpdateAsync(ingrediente);

        _mockCollection.Verify(c => c.ReplaceOneAsync(
            It.IsAny<FilterDefinition<Ingrediente>>(),
            ingrediente,
            It.IsAny<ReplaceOptions>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task DeleteAsync_CallsDeleteOne()
    {
        await _repository.DeleteAsync("1");

        _mockCollection.Verify(c => c.DeleteOneAsync(
            It.IsAny<FilterDefinition<Ingrediente>>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

}
