using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Configuration;
using pedidos_service.Application.Interfaces;
using pedidos_service.Application.Services;
using pedidos_service.Domain.Repositories;
using pedidos_service.Domain.Services;
using pedidos_service.Infraestructure.Persistence.MongoDB;
using pedidos_service.Infraestructure.Persistence.Repositories;
using pedidos_service.Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

//builder.WebHost.ConfigureKestrel(serverOptions =>
//{
//    serverOptions.ListenAnyIP(8080); // Para que funcione en Docker sin problemas
//});
builder.WebHost.UseUrls("http://0.0.0.0:8080");

// Configuración de MongoDB
var mongoDbSettings = builder.Configuration.GetSection("MongoDBSettings");
builder.Services.Configure<MongoDBSettings>(mongoDbSettings);

// Configuración de Kafka
builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));

// Inyeccion de dependencias
builder.Services.AddSingleton<IMongoDBContext, MongoDBContext>(); 
builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>(); 
builder.Services.AddHostedService<PedidoConsumer>(); 

// Servicios del dominio
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<ICreacionPedidoService, CreacionPedidoService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();

// Configuración de Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Microservicio de Pedidos",
        Version = "v1",
        Description = "Microservicio de pedidos - construccion de software"
    });
});

var app = builder.Build();

// Configuración de Swagger en entorno de desarrollo
if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
