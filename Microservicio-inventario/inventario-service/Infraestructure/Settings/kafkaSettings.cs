namespace inventario_service.Infrastructure.Settings
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; }
        public KafkaTopicSettings Topic { get; set; }
        public string GroupId { get; set; }
    }

    public class KafkaTopicSettings
    {
        public string PedidoCreado { get; set; }
    }
}
