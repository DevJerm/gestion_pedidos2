namespace pedidos_service.Domain.ValueObjects
{
    public class TotalPedido
    {
        public decimal Valor { get; }

        public TotalPedido(decimal valor)
        {
            if (valor < 0)
                throw new ArgumentException("El total del pedido no puede ser negativo", nameof(valor));

            Valor = valor;
        }
    }
}
