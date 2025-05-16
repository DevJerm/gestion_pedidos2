namespace pedidos_service.Domain.ValueObjects
{
    public class DireccionEntrega
    {
        // lo llamamos calle referencia a Calle o carrera que se utiliza en Colombia 
        public string Calle { get; }
        public string Numero { get; }
        public string Ciudad { get; }
        public string CodigoPostal { get; }
        public string Referencia { get; }

        public DireccionEntrega(string calle, string numero, string ciudad, string codigoPostal, string referencia = "")
        {
            if (string.IsNullOrWhiteSpace(calle))
                throw new ArgumentException("La calle/carrera no puede estar vacía", nameof(calle));

            if (string.IsNullOrWhiteSpace(numero))
                throw new ArgumentException("El número no puede estar vacío", nameof(numero));

            if (string.IsNullOrWhiteSpace(ciudad))
                throw new ArgumentException("La ciudad no puede estar vacía", nameof(ciudad));

            if (string.IsNullOrWhiteSpace(codigoPostal))
                throw new ArgumentException("El código postal no puede estar vacío", nameof(codigoPostal));

            Calle = calle;
            Numero = numero;
            Ciudad = ciudad;
            CodigoPostal = codigoPostal;
            Referencia = referencia;
        }

        public override string ToString()
        {
            return $"{Calle} {Numero}, {Ciudad}, CP: {CodigoPostal} {(string.IsNullOrEmpty(Referencia) ? "" : $"- Ref: {Referencia}")}";
        }
    }
}