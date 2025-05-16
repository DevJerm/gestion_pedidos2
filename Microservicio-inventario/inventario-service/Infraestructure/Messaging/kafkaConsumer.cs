using Confluent.Kafka;
using inventario_service.Domain.Services;
using inventario_service.Domain.Repositories;
using inventario_service.Application.Events;
using inventario_service.Infrastructure.Messaging;
using Newtonsoft.Json;

public class PedidoConsumer : BackgroundService
{
    private readonly ILogger<PedidoConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly KafkaProducer _kafkaProducer;

    public PedidoConsumer(
        ILogger<PedidoConsumer> logger,
        IServiceScopeFactory scopeFactory,
        KafkaProducer kafkaProducer)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _kafkaProducer = kafkaProducer;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Run(async () =>
        {
            using var consumer = new ConsumerBuilder<Ignore, string>(new ConsumerConfig
            {
                BootstrapServers = "kafka:9092",
                GroupId = "inventario-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                SocketTimeoutMs = 5000,
                SessionTimeoutMs = 6000,
                EnableAutoCommit = true
            }).Build();

            try
            {
                consumer.Subscribe("pedido-creado");
                _logger.LogInformation("suscrito a  'pedido-creado'");

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var result = consumer.Consume(TimeSpan.FromMilliseconds(100));
                        if (result == null) continue;

                        await HandleMessageAsync(result.Message.Value, stoppingToken);
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError($"Kafka error: {ex.Error.Reason}");
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError($"JSON error: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"error: {ex.Message}");
                    }

                    await Task.Delay(100, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al iniciar consumer: {ex.Message}");
            }
            finally
            {
                consumer.Close();
            }
        }, stoppingToken);

        return Task.CompletedTask;
    }

    private async Task HandleMessageAsync(string mensaje, CancellationToken stoppingToken)
    {
        var evento = JsonConvert.DeserializeObject<PedidoCreadoEvent>(mensaje);

        using var scope = _scopeFactory.CreateScope();
        var recetaRepository = scope.ServiceProvider.GetRequiredService<IRecetaRepository>();
        var validacionService = scope.ServiceProvider.GetRequiredService<IValidacionDisponibilidadService>();
        var inventarioService = scope.ServiceProvider.GetRequiredService<IInventarioService>();

        foreach (var item in evento.Items)
        {
            var receta = await recetaRepository.GetByIdAsync(item.ProductoId);
            if (receta == null)
            {
                _logger.LogWarning($"Receta con ID {item.ProductoId} no encontrada");
                await _kafkaProducer.PublicarAsync("disponibilidad-validada", new DisponibilidadValidadaEvent
                {
                    PedidoId = evento.PedidoId,
                    Disponible = false
                });
                continue;
            }

            var ingredientesRequeridos = receta.Ingredientes.Select(i => new IngredienteRequerido
            {
                IngredienteId = i.IngredienteId,
                CantidadRequerida = i.Cantidad * item.Cantidad
            }).ToList();

            var disponible = await validacionService.ValidarDisponibilidadIngredientes(ingredientesRequeridos);

            if (disponible)
            {
                await inventarioService.ActualizarInventario(ingredientesRequeridos);
                await _kafkaProducer.PublicarAsync("disponibilidad-validada", new DisponibilidadValidadaEvent
                {
                    PedidoId = evento.PedidoId,
                    Disponible = true
                });
                _logger.LogInformation($"Inventario actualizado para receta del producto {item.ProductoId}");
            }
            else
            {
                _logger.LogWarning($"Ingredientes insuficientes para receta del producto {item.ProductoId}");
                await _kafkaProducer.PublicarAsync("disponibilidad-validada", new DisponibilidadValidadaEvent
                {
                    PedidoId = evento.PedidoId,
                    Disponible = false
                });
            }
        }
    }
}
