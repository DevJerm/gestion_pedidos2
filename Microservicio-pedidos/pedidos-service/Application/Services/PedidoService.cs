using pedidos_service.Application.DTOs;
using pedidos_service.Application.Interfaces;
using pedidos_service.Domain.Entities;
using pedidos_service.Application.Events;
using pedidos_service.Domain.Repositories;
using pedidos_service.Domain.Services;
using pedidos_service.Domain.ValueObjects;
namespace pedidos_service.Application.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly ICreacionPedidoService _creacionPedidoService;

        private readonly IKafkaProducer _kafkaProducer;
        // utilizamos inversion de dependencias, para no depender de implementaciones sinoi de abstracciones. La inyeccion se hace en el startup 
        public PedidoService(
            IPedidoRepository pedidoRepository,
            IClienteRepository clienteRepository,
            ICreacionPedidoService creacionPedidoService,
            IKafkaProducer kafkaProducer
            )
        {
            _pedidoRepository = pedidoRepository;
            _clienteRepository = clienteRepository;
            _creacionPedidoService = creacionPedidoService;
            _kafkaProducer = kafkaProducer
            ;
        }

        public async Task<PedidoDTO> CrearPedidoAsync(CrearPedidoDTO crearPedidoDTO)
        {
            // verificar que el cliente existe sino lanzar excepcion de no encontrado.
            var cliente = await _clienteRepository.GetByIdAsync(crearPedidoDTO.ClienteId);
            if (cliente == null)
                throw new KeyNotFoundException($"Cliente con ID {crearPedidoDTO.ClienteId} no encontrado");

            // convertir DTO a objetos de dominio para crear  pedido
            var direccionEntrega = new DireccionEntrega(
                crearPedidoDTO.DireccionEntrega.Calle,
                crearPedidoDTO.DireccionEntrega.Numero,
                crearPedidoDTO.DireccionEntrega.Ciudad,
                crearPedidoDTO.DireccionEntrega.CodigoPostal,
                crearPedidoDTO.DireccionEntrega.Referencia);

            var items = crearPedidoDTO.Items.Select(i =>
                (i.ProductoId, i.Cantidad, i.PrecioUnitario)).ToList();

            // crea pedido usando el servicio de dominio
            var pedido = await _creacionPedidoService.CrearPedidoAsync(
                crearPedidoDTO.ClienteId,
                direccionEntrega,
                items);

            // persistir el pedido
            await _pedidoRepository.AddAsync(pedido);


            var pedidoCreado = new PedidoCreado
            {
                PedidoId = pedido.Id,
                ClienteId = pedido.ClienteId,
                FechaCreacion = pedido.FechaCreacion,
                Items = pedido.Items.Select(i => new PedidoCreado.ItemPedidoEvent
                {
                    ProductoId = i.ProductoId,
                    Cantidad = i.Cantidad
                }).ToList()
            };
            await _kafkaProducer.PublishAsync("pedido-creado", pedidoCreado);


            return MapToDTO(pedido);
        }

        public async Task<PedidoDTO> GetPedidoByIdAsync(string pedidoId)
        {
            var pedido = await _pedidoRepository.GetByIdAsync(pedidoId);
            if (pedido == null)
                throw new KeyNotFoundException($"Pedido con ID {pedidoId} no encontrado");

            return MapToDTO(pedido);
        }

        public async Task<IEnumerable<PedidoDTO>> GetPedidosByClienteIdAsync(string clienteId)
        {
            var pedidos = await _pedidoRepository.GetByClienteIdAsync(clienteId);
            return pedidos.Select(MapToDTO);
        }

        public async Task<IEnumerable<PedidoDTO>> GetPedidosByEstadoAsync(string estado)
        {
            if (!Enum.TryParse<EstadoPedido>(estado, true, out var estadoPedido))
                throw new ArgumentException($"Estado de pedido inválido: {estado}");

            var pedidos = await _pedidoRepository.GetByEstadoAsync(estadoPedido);
            return pedidos.Select(MapToDTO);
        }

        public async Task<PedidoDTO> ActualizarEstadoPedidoAsync(string pedidoId, string nuevoEstado)
        {
            var pedido = await _pedidoRepository.GetByIdAsync(pedidoId);
            if (pedido == null)
                throw new KeyNotFoundException($"Pedido con ID {pedidoId} no encontrado");

            //utilizamos un enum para no sobre diseñar la solucion, de acuerdo a las recomendaciones tambien realizadas en clase de ing Software con el profe Jhonatan

            switch (nuevoEstado.ToUpper())
            {
                case "CONFIRMADO":
                    pedido.ConfirmarPedido();
                    break;
                case "EN_PREPARACION":
                    pedido.MarcarEnPreparacion();
                    break;
                case "LISTO":
                    pedido.MarcarListo();
                    break;
                case "ENTREGADO":
                    pedido.MarcarEntregado();
                    break;
                case "SIN_STOCK":
                    pedido.MarcarSinStock();
                    break;
                default:
                    throw new ArgumentException($"Estado de pedido no válido: {nuevoEstado}");
            }

            await _pedidoRepository.UpdateAsync(pedido);
            return MapToDTO(pedido);
        }


        //dejamos este DTO privado para solo utilizarlo en esta clase. Pero puede quedar tambien en la carpeta dtos pero "publicoo"
        private PedidoDTO MapToDTO(Pedido pedido)
        {
            return new PedidoDTO
            {
                Id = pedido.Id,
                ClienteId = pedido.ClienteId,
                Estado = pedido.Estado.ToString(),
                FechaCreacion = pedido.FechaCreacion,
                Total = pedido.Total.Valor,
                DireccionEntrega = new DireccionEntregaDTO
                {
                    Calle = pedido.DireccionEntrega.Calle,
                    Numero = pedido.DireccionEntrega.Numero,
                    Ciudad = pedido.DireccionEntrega.Ciudad,
                    CodigoPostal = pedido.DireccionEntrega.CodigoPostal,
                    Referencia = pedido.DireccionEntrega.Referencia
                },
                Items = pedido.Items.Select(i => new ItemPedidoDTO
                {
                    ProductoId = i.ProductoId,
                    Cantidad = i.Cantidad,
                    PrecioUnitario = i.PrecioUnitario
                }).ToList()
            };
        }
    }
}

