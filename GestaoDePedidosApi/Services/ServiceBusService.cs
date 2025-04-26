using Azure.Messaging.ServiceBus;
using GestaoDesPedidosApi.Models;
using System.Text.Json;
using System.Threading.Tasks;

namespace GestaoDesPedidosApi.Services
{
    public class ServiceBusService
    {
        private readonly string _connectionString;
        private readonly string _queueName;

        public ServiceBusService(string connectionString, string queueName)
        {
            _connectionString = connectionString;
            _queueName = queueName;
        }

        public async Task SendOrderMessageAsync(Order order)
        {
            // Create a client
            await using var client = new ServiceBusClient(_connectionString);
            
            // Create a sender
            ServiceBusSender sender = client.CreateSender(_queueName);
            
            // Serialize the order to JSON
            string messageBody = JsonSerializer.Serialize(order);
            
            // Create a message
            ServiceBusMessage message = new ServiceBusMessage(messageBody);
            
            // Send the message
            await sender.SendMessageAsync(message);
        }
    }
}
