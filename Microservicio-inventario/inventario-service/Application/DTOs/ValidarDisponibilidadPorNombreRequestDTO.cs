namespace inventario_service.Application.DTOs
{
    public class ValidarDisponibilidadPorNombreRequestDTO
    {
        public List<IngredienteRequeridoPorNombre> Ingredientes { get; set; }
    }

    public class IngredienteRequeridoPorNombre
    {
        public string Nombre { get; set; }
        public double CantidadRequerida { get; set; }
    }
}
