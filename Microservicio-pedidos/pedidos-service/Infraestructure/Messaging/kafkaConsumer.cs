using Confluent.Kafka;
using Newtonsoft.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using pedidos_service.Application.Interfaces;
using pedidos_service.Application.Events;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using inventario_service.Application.Events;

public class PedidoConsumer : BackgroundService
{
    private readonly ILogger<PedidoConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly string _kafkaBootstrapServers = "kafka:9092";  

    public PedidoConsumer(ILogger<PedidoConsumer> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() =>
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _kafkaBootstrapServers,
                GroupId = "pedido-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
            {
                consumer.Subscribe("disponibilidad-validada");

                _logger.LogInformation("PedidoConsumer suscrito a disponibilidad-validada");

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(stoppingToken);
                        var message = consumeResult.Message.Value;

                        var evento = JsonConvert.DeserializeObject<DisponibilidadValidadaEvent>(message);

                        if (evento != null)
                        {
                            ProcessEventAsync(evento).GetAwaiter().GetResult();
                        }
                    }
                    catch (ConsumeException e)
                    {
                        _logger.LogError($"Error consuming message: {e.Error.Reason}");
                    }
                    catch (JsonException e)
                    {
                        _logger.LogError($"Error deserializing message: {e.Message}");
                    }
                }
            }
        }, stoppingToken);
    }


    private async Task ProcessEventAsync(DisponibilidadValidadaEvent evento)
    {
        try
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var pedidoService = scope.ServiceProvider.GetRequiredService<IPedidoService>();

                if (evento.Disponible)
                {
                    await pedidoService.ActualizarEstadoPedidoAsync(evento.PedidoId, "CONFIRMADO");
                    _logger.LogInformation($"Pedido confirmado: {evento.PedidoId}");
                }
                else
                {
                    await pedidoService.ActualizarEstadoPedidoAsync(evento.PedidoId, "SIN_STOCK");
                    _logger.LogWarning($"Pedido con ID {evento.PedidoId} sin stock");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error processing event: {ex.Message}");
        }
    }
}
