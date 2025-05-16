using System;
using Xunit;
using pedidos_service.Domain.Entities;
using pedidos_service.Domain.ValueObjects;
using pedidos_service.Application.DTOs;

namespace pedidos_service.tests.Unitarias
{
    public class PedidoTests
    {

        private readonly DireccionEntrega _direccionPrueba = new DireccionEntrega(
            "Calle 33", "123", "medellin", "12345", "Cerca del parque");


        [Fact]
        public void AgregarItem_DebeCalcularTotalCorrectamente()
        {
            // Arrange
            var pedido = new Pedido("cliente123", _direccionPrueba);

            // Act
            pedido.AgregarItem("producto1", 2, 10);
            pedido.AgregarItem("producto2", 1, 15);

            // Assert
            Assert.Equal(2, pedido.Items.Count);
            Assert.Equal(35, pedido.Total.Valor);
        }

        [Fact]
        public void ConfirmarPedido_CambiaEstadoCorrectamente()
        {
            // Arrange
            var pedido = new Pedido("cliente123", _direccionPrueba);

            // Act
            pedido.ConfirmarPedido();

            // Assert
            Assert.Equal(EstadoPedido.CONFIRMADO, pedido.Estado);
        }

        [Fact]
        public void MarcarEnPreparacion_LanzaExcepcionSiNoEstaConfirmado()
        {
            // Arrange
            var pedido = new Pedido("cliente123", _direccionPrueba);

            // Act y Assert
            var ex = Assert.Throws<InvalidOperationException>(() => pedido.MarcarEnPreparacion());
            Assert.Contains("CONFIRMADOS", ex.Message);
        }

        [Fact]
        public void FlujoCompleto_DebePermitirCambiosDeEstadoEnOrden()
        {
            // Arrange
            var pedido = new Pedido("cliente123", _direccionPrueba);

            // Act y Assert
            pedido.ConfirmarPedido();
            Assert.Equal(EstadoPedido.CONFIRMADO, pedido.Estado);

            pedido.MarcarEnPreparacion();
            Assert.Equal(EstadoPedido.EN_PREPARACION, pedido.Estado);

            pedido.MarcarListo();
            Assert.Equal(EstadoPedido.LISTO, pedido.Estado);

            pedido.MarcarEntregado();
            Assert.Equal(EstadoPedido.ENTREGADO, pedido.Estado);
        }

        [Fact]
        public void ConfirmarPedido_LanzaExcepcion_SiNoEstaEnCreado()
        {
            var pedido = new Pedido("cliente123", _direccionPrueba);
            pedido.ConfirmarPedido(); // cambia a CONFIRMADO

            var ex = Assert.Throws<InvalidOperationException>(() => pedido.ConfirmarPedido());
            Assert.Contains("estado CREADO", ex.Message);
        }

        [Fact]
        public void MarcarEnPreparacion_CambiaEstado_SiEstaConfirmado()
        {
            var pedido = new Pedido("cliente123", _direccionPrueba);
            pedido.ConfirmarPedido();

            pedido.MarcarEnPreparacion();

            Assert.Equal(EstadoPedido.EN_PREPARACION, pedido.Estado);
        }

        [Fact]
        public void MarcarListo_LanzaExcepcion_SiNoEstaEnPreparacion()
        {
            var pedido = new Pedido("cliente123", _direccionPrueba);
            pedido.ConfirmarPedido(); // Estado: CONFIRMADO

            var ex = Assert.Throws<InvalidOperationException>(() => pedido.MarcarListo());
            Assert.Contains("EN_PREPARACION", ex.Message);
        }

        [Fact]
        public void MarcarEntregado_LanzaExcepcion_SiNoEstaListo()
        {
            var pedido = new Pedido("cliente123", _direccionPrueba);
            pedido.ConfirmarPedido();
            pedido.MarcarEnPreparacion(); // Estado: EN_PREPARACION

            var ex = Assert.Throws<InvalidOperationException>(() => pedido.MarcarEntregado());
            Assert.Contains("LISTOS", ex.Message);
        }

        [Fact]
        public void RecalcularTotal_ActualizaCorrectamenteConMultiplesItems()
        {
            var pedido = new Pedido("cliente456", _direccionPrueba);
            pedido.AgregarItem("producto1", 3, 20);  // 60
            pedido.AgregarItem("producto2", 2, 15);  // 30
            pedido.AgregarItem("producto3", 1, 10);  // 10

            Assert.Equal(100, pedido.Total.Valor);
        }      
    }
}
