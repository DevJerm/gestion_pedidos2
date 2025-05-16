using Confluent.Kafka;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using pedidos_service.Application.Interfaces;

public class KafkaProducer : IKafkaProducer, IDisposable
{
    private readonly IProducer<Null, string> _producer;
    private readonly ILogger<KafkaProducer> _logger;

    public KafkaProducer(IConfiguration configuration, ILogger<KafkaProducer> logger)
    {
        _logger = logger;

        var bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "kafka:9092";
        _logger.LogInformation($"Kafka Producer initialized with server: {bootstrapServers}");

        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            Acks = Acks.All,
            EnableIdempotence = true,
            MessageSendMaxRetries = 3,
            RetryBackoffMs = 1000
        };

        _producer = new ProducerBuilder<Null, string>(config)
            .SetLogHandler((_, message) => _logger.LogInformation($"Kafka Producer Log: {message.Message}"))
            .SetErrorHandler((_, error) => _logger.LogError($"Kafka Producer Error: {error.Reason}"))
            .Build();
    }

    public async Task PublishAsync<T>(string topic, T message)
    {
        try
        {
            var jsonMessage = JsonConvert.SerializeObject(message);
            var kafkaMessage = new Message<Null, string> { Value = jsonMessage };

            var result = await _producer.ProduceAsync(topic, kafkaMessage);
            _logger.LogInformation($"Message sent to Kafka topic '{result.Topic}', Partition: {result.Partition}, Offset: {result.Offset}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error publishing message to Kafka: {ex.Message}");
            throw;
        }
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }
}
