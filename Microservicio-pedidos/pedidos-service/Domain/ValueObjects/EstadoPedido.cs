namespace pedidos_service.Domain.ValueObjects
{
    //enum para no tener que hacer varias clases de cada tipo de pedido que herende de una clase padre.
    //Todas tienen las mismas reglas, considero no necesario. Recomendado tambien en clase de Arq Software con Jonathan
    public enum EstadoPedido
    {
        CREADO,
        CONFIRMADO,
        EN_PREPARACION,
        LISTO,
        ENTREGADO,
        SIN_STOCK
    }
}
