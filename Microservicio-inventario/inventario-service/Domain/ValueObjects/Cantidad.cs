namespace inventario_service.Domain.ValueObjects
{
    public class Cantidad
    {
        public double Valor { get; private set; }
        public string UnidadMedida { get; private set; }

        public Cantidad(double valor, string unidadMedida)
        {
            Valor = valor;
            UnidadMedida = unidadMedida;
        }
    }
}