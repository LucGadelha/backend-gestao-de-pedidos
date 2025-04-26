using Azure.Messaging.ServiceBus;
using GestaoDesPedidosApi.Data;
using GestaoDesPedidosApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoDesPedidosApi.Services
{
    public class OrderProcessingService : BackgroundService
    {
        private readonly ILogger<OrderProcessingService> _logger;
        private readonly ServiceBusClient _client;
        private readonly ServiceBusProcessor _processor;
        private readonly IServiceScopeFactory _scopeFactory;

        public OrderProcessingService(
            ILogger<OrderProcessingService> logger,
            string connectionString,
            string queueName,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;

            // Create the clients that we'll use for sending and processing messages.
            _client = new ServiceBusClient(connectionString);
            _processor = _client.CreateProcessor(queueName, new ServiceBusProcessorOptions());

            // Add handler to process messages
            _processor.ProcessMessageAsync += MessageHandler;

            // Add handler to process any errors
            _processor.ProcessErrorAsync += ErrorHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Start processing
            await _processor.StartProcessingAsync(stoppingToken);
            
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }
            }
            finally
            {
                // Stop processing 
                await _processor.StopProcessingAsync(stoppingToken);
                await _processor.DisposeAsync();
                await _client.DisposeAsync();
            }
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            try
            {
                string body = args.Message.Body.ToString();
                var order = JsonSerializer.Deserialize<Order>(body);
                
                if (order != null)
                {
                    _logger.LogInformation($"Received message for Order: {order.Id}");
                    
                    // Update order status to "Processando"
                    await UpdateOrderStatus(order.Id, "Processando");
                    
                    // Wait 5 seconds
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    
                    // Update order status to "Finalizado"
                    await UpdateOrderStatus(order.Id, "Finalizado");
                }
                
                // Complete the message
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Service Bus message");
                // Abandoning the message will make it available for processing again after the lock expires
                await args.AbandonMessageAsync(args.Message);
            }
        }

        private async Task UpdateOrderStatus(Guid orderId, string status)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
            
            var order = await dbContext.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                await dbContext.SaveChangesAsync();
                _logger.LogInformation($"Updated Order {orderId} status to: {status}");
            }
            else
            {
                _logger.LogWarning($"Order {orderId} not found when trying to update status to {status}");
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, "Error handling Service Bus message");
            return Task.CompletedTask;
        }
    }
}
