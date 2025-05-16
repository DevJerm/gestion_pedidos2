namespace pedidos_service.Domain.Entities;

public class ItemPedido
{
    public string Id { get; private set; }
    public string ProductoId { get; private set; }
    public int Cantidad { get; private set; }
    public decimal PrecioUnitario { get; private set; }
    public decimal Subtotal => Cantidad * PrecioUnitario;

    public ItemPedido(string productoId, int cantidad, decimal precioUnitario)
    {
        Id = Guid.NewGuid().ToString();
        ProductoId = productoId;
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
    }
}
