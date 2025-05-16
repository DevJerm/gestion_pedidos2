using pedidos_service.Application.DTOs;
using pedidos_service.Domain.Entities;
using pedidos_service.Domain.ValueObjects;
using Xunit;

public class ClienteTests
{
    [Fact]
    public void Constructor_ValoresValidos_CreaClienteCorrectamente()
    {
        // Arrange
        var nombre = "Juan Pérez";
        var email = "juan@example.com";
        var telefono = "3001234567";
        var direccion = new DireccionEntrega("Calle33", "47-35", "Medellin", "050000","parquesito");

        // Act
        var cliente = new Cliente(nombre, email, telefono, direccion);

        // Assert
        Assert.NotNull(cliente.Id);
        Assert.Equal(nombre, cliente.Nombre);
        Assert.Equal(email, cliente.Email);
        Assert.Equal(telefono, cliente.Telefono);
        Assert.Equal(direccion, cliente.DireccionPredeterminada);
    }

    [Fact]
    public void Constructor_GeneraIdUnico()
    {
        // Arrange
        var direccion = new DireccionEntrega("Calle33", "47-35", "Medellin", "050000", "parquesito");

        // Act
        var cliente1 = new Cliente("Cliente1", "c1@mail.com", "123", direccion);
        var cliente2 = new Cliente("Cliente2", "c2@mail.com", "456", direccion);

        // Assert
        Assert.NotEqual(cliente1.Id, cliente2.Id);
    }

    [Fact]
    public void DireccionPredeterminada_ValoresCorrectos()
    {
        // Arrange
        var direccion = new DireccionEntrega("Calle33", "47-35", "Medellin", "050000", "parquesito");

        // Act
        var cliente = new Cliente("Pepito perez", "ppppp@mail.com", "310000000", direccion);

        // Assert
        Assert.Equal("Calle33", cliente.DireccionPredeterminada.Calle);
        Assert.Equal("47-35", cliente.DireccionPredeterminada.Numero);
        Assert.Equal("Medellin", cliente.DireccionPredeterminada.Ciudad);
        Assert.Equal("050000", cliente.DireccionPredeterminada.CodigoPostal);
        Assert.Equal("parquesito", cliente.DireccionPredeterminada.Referencia);
    }


}
