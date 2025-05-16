using pedidos_service.Domain.ValueObjects;
using Xunit;

namespace pedidos_service.Tests.Domain.ValueObjects
{
    public class DireccionEntregaTests
    {
        [Fact]
        public void Constructor_ValoresValidos_SinReferencia_CreaObjetoCorrectamente()
        {
            // Arrange Act
            var direccion = new DireccionEntrega("Cr33", "47-35", "Medellin", "05000");

            // Assert
            Assert.Equal("Cr33", direccion.Calle);
            Assert.Equal("47-35", direccion.Numero);
            Assert.Equal("Medellin", direccion.Ciudad);
            Assert.Equal("05000", direccion.CodigoPostal);
            Assert.Equal("", direccion.Referencia);
        }

        [Fact]
        public void Constructor_ValoresValidos_ConReferencia_CreaObjetoCorrectamente()
        {
            var direccion = new DireccionEntrega("Cr33", "47-35", "Medellin", "05000", "buenos a");

            Assert.Equal("Cr33", direccion.Calle);
            Assert.Equal("47-35", direccion.Numero);
            Assert.Equal("Medellin", direccion.Ciudad);
            Assert.Equal("05000", direccion.CodigoPostal);
            Assert.Equal("buenos a", direccion.Referencia);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_CalleInvalida_LanzaExcepcion(string calleInvalida)
        {
            var ex = Assert.Throws<ArgumentException>(() => new DireccionEntrega(calleInvalida, "123", "Medallo", "11111"));
            Assert.Equal("La calle/carrera no puede estar vacía (Parameter 'calle')", ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_NumeroInvalido_LanzaExcepcion(string numeroInvalido)
        {
            var ex = Assert.Throws<ArgumentException>(() => new DireccionEntrega("Cll33", numeroInvalido, "medellin", "111111"));
            Assert.Equal("El número no puede estar vacío (Parameter 'numero')", ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_CiudadInvalida_LanzaExcepcion(string ciudadInvalida)
        {
            var ex = Assert.Throws<ArgumentException>(() => new DireccionEntrega("Cr33", "47-35", ciudadInvalida, "111111"));
            Assert.Equal("La ciudad no puede estar vacía (Parameter 'ciudad')", ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_CodigoPostalInvalido_LanzaExcepcion(string codigoPostalInvalido)
        {
            var ex = Assert.Throws<ArgumentException>(() => new DireccionEntrega("Cr33", "4735", "medellin", codigoPostalInvalido));
            Assert.Equal("El código postal no puede estar vacío (Parameter 'codigoPostal')", ex.Message);
        }

        [Fact]
        public void ToString_SinReferencia_DevuelveFormatoCorrecto()
        {
            var direccion = new DireccionEntrega("Cr33", "47-35", "Medellin", "05000");

            var resultado = direccion.ToString();

            Assert.Equal("Cr33 47-35, Medellin, CP: 05000 ", resultado);
        }

        [Fact]
        public void ToString_ConReferencia_DevuelveFormatoCorrecto()
        {
            var direccion = new DireccionEntrega("Cr33", "47-35", "Medellin", "05000","Buenos Aires");

            var resultado = direccion.ToString();

            Assert.Equal("Cr33 47-35, Medellin, CP: 05000 - Ref: Buenos Aires", resultado);
        }
    }
}