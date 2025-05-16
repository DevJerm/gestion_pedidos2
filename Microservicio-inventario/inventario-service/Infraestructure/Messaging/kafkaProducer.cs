using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using inventario_service.Infrastructure.Settings;

namespace inventario_service.Infrastructure.Messaging
{
    public class KafkaProducer
    {
        private readonly IProducer<Null, string> _producer;

        public KafkaProducer(IOptions<KafkaSettings> settings)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = settings.Value.BootstrapServers
            };

            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task PublicarAsync<T>(string topic, T data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var message = new Message<Null, string> { Value = json };

                var result = await _producer.ProduceAsync(topic, message);
                Console.WriteLine($"Mensaje publicado en Kafka: {topic} [{result.TopicPartitionOffset}]");
            }
            catch (ProduceException<Null, string> ex)
            {
                Console.WriteLine($"Error al publicar en Kafka: {ex.Error.Reason}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al publicar en Kafka: {ex.Message}");
                throw;
            }
        }

    }
}
