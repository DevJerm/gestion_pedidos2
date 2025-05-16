namespace inventario_service.Application.DTOs
{
    public class IngredienteDTO
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public double Cantidad { get; set; }
        public string UnidadMedida { get; set; }
        public bool Disponible { get; set; }
    }
    //public class IngredienteDTO_POST
    //{
    //    public string Nombre { get; set; }
    //    public double Cantidad { get; set; }
    //    public string UnidadMedida { get; set; }
    //    public bool Disponible { get; set; }
    //}
}