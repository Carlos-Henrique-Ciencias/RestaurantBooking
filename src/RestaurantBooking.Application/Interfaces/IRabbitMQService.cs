namespace RestaurantBooking.Application.Interfaces;

public interface IRabbitMQService
{
    void Publish(object message, string queueName);
}
