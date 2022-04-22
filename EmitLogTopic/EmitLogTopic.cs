//Working
//dotnet run "kern.critical" "A critical kernel error"
using RabbitMQ.Client;
using System.Text;

Console.WriteLine("Hello, World!");
//var qName = "My_Tasks";
var exchangeName = "topic_logs";

var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);

var routingKey = (args.Length > 0) ? args[0] : "anonymous.info";

//channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

 
string message = GetMessage(args);
var body = Encoding.UTF8.GetBytes(message);

//var properties = channel.CreateBasicProperties();
//properties.Persistent = true;

channel.BasicPublish(
    exchange: exchangeName,
    routingKey: routingKey,
    basicProperties: null,
    body: body
    );
Console.WriteLine("[X] sebt {0}", message);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();

static string GetMessage(string[] args)
{
    var message = "Hello Eorld";
    if (args.Length > 1)
    {
        message = string.Join(" ", args.Skip(1).ToArray());
    }
    return message;
}