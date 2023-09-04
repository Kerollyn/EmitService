using System.Text;
using System.Text.Json;
using EmailService.ProcessEvent;
using EmitService.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EmitService.AsyncDataServices
{
  public class MessageBusClient : IMessageBusClient
  {
    private readonly IConfiguration _configuration;
    private readonly ILogger<MessageBusClient> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageBusClient(IConfiguration configuration,
    ILogger<MessageBusClient> logger)
    {
      _configuration = configuration;
      _logger = logger;

      var factory = new ConnectionFactory()
      {
        HostName = _configuration["RabbitMQHost"],
        Port = int.Parse(_configuration["RabbitMQPort"])
      };

      try
      {
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

        _channel.QueueDeclare(queue: nameof(Email),
                              durable: true,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);

        _logger.LogInformation("--> Connected to MessegeBus");
      }
      catch (Exception e)
      {
        _logger.LogError($"--> Could not connect to the Message Bus: {e.Message} ");
      }
    }

    public void PublishNewEmailDocument(Email email)
    {
      var message = JsonSerializer.Serialize(email);

      if (_connection.IsOpen)
      {
        _logger.LogInformation("--> RabbitMQ Connection Open, sending message ...");

        var replayQueue = $"{nameof(Email)}_return";
        _channel.QueueDeclare(queue: replayQueue,
                              durable: true,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
          var body = ea.Body.ToArray();
          var message = Encoding.UTF8.GetString(body);
          return;
        };

        _channel.BasicConsume(queue: replayQueue,
                              autoAck: true,
                              consumer: consumer);

        var pros = _channel.CreateBasicProperties();
        pros.ReplyTo = replayQueue;

        var body = Encoding.UTF8.GetBytes(message);
        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;

        _channel.BasicPublish(exchange: "",
                              routingKey: nameof(Email),
                              basicProperties: properties,
                              body: body);
      }
      else
      {
        _logger.LogError("--> RabbitMQ Connection Closed, not sending");
      }
    }

    public void Dispose()
    {
      Console.WriteLine("Message Disposed");

      if (_channel.IsOpen)
      {
        _channel.Close();
        _connection.Close();
      }
    }
    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
      _logger.LogInformation("--> RabbitMQ Connection Shutdown");
    }

  }
}

