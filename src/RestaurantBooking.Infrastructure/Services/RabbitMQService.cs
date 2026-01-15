using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RestaurantBooking.Application.Interfaces;

namespace RestaurantBooking.Infrastructure.Services;

public class RabbitMQService : IRabbitMQService
{
    private readonly string _connectionString;

    public RabbitMQService(IConfiguration configuration)
    {
        // Garante que não é nulo. Se for, usa um fallback local
        _connectionString = configuration.GetConnectionString("RabbitMQ") 
                            ?? "amqp://booking:booking123@localhost:5672";
    }

    public void Publish(object message, string queueName)
    {
        var factory = new ConnectionFactory { Uri = new Uri(_connectionString) };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: queueName,
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish(exchange: "",
                             routingKey: queueName,
                             basicProperties: null,
                             body: body);
    }
}
