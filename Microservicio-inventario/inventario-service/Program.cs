using inventario_service.Application.Services;
using inventario_service.Domain.Repositories;
using inventario_service.Domain.Services;
using inventario_service.Infraestructure.Persistence.MongoDB;
using inventario_service.Infraestructure.Persistence.Repositories;
using inventario_service.Infrastructure.Messaging;
using inventario_service.Infrastructure.Persistence.MongoDB;
using inventario_service.Infrastructure.Settings;
using Microsoft.OpenApi.Models;
using Microsoft.Win32;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:8080");
//builder.WebHost.ConfigureKestrel(serverOptions =>
//{
//    serverOptions.ListenAnyIP(8080); // Para que funcione en Docker sin problemas
//});

builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDBSettings"));

//Registro de servicio de MongoDB. Singleton para que solo haya una instancia de MongoDBContext.
builder.Services.AddSingleton<IMongoDBContext, MongoDBContext>();

//inyecciones de dependecias 
builder.Services.AddScoped<IIngredienteRepository, IngredienteRepository>();
builder.Services.AddScoped<IRecetaRepository, RecetaRepository>();
builder.Services.AddScoped<IValidacionDisponibilidadService, ValidacionDisponibilidadService>();
builder.Services.AddScoped<IInventarioService, InventarioService>();
builder.Services.AddScoped<IRecetaService, RecetaService>();
builder.Services.AddScoped<InventarioService>();
builder.Services.AddScoped<RecetaService>();

builder.Services.Configure<KafkaSettings>(
    builder.Configuration.GetSection("Kafka"));
builder.Services.AddSingleton<KafkaProducer>();
builder.Services.AddHostedService<PedidoConsumer>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Microservicio de inventario",
        Version = "v1",
        Description = "Microservicio de inventario - Construccion de software"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();