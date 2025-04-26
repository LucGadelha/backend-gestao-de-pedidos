using GestaoDesPedidosApi.Data;
using GestaoDesPedidosApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.WithOrigins("http://localhost:3000") // Porta padrão do desenvolvimento React/Next.js
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add Entity Framework Core with PostgreSQL
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Azure Service Bus
string serviceBusConnectionString = builder.Configuration.GetValue<string>("AzureServiceBus:ConnectionString") ?? "Endpoint=sb://your-servicebus-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=your-key";
string queueName = builder.Configuration.GetValue<string>("AzureServiceBus:QueueName") ?? "orders-queue";

// Register ServiceBusService
builder.Services.AddSingleton(provider => new ServiceBusService(serviceBusConnectionString, queueName));

// Register Order Processing Background Service
builder.Services.AddHostedService(provider => new OrderProcessingService(
    provider.GetRequiredService<ILogger<OrderProcessingService>>(),
    serviceBusConnectionString,
    queueName,
    provider.GetRequiredService<IServiceScopeFactory>()));

// Add OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Gestão de Pedidos API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowFrontend");

app.UseAuthorization();
app.MapControllers();

app.Run();
