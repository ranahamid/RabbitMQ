//dotnet run "kern.*" "*.critical"
//dotnet run "*.critical"
//dotnet run "kern.*"
//dotnet run "#"
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

Console.WriteLine("Hello, World!");
var factory = new ConnectionFactory()
{
    HostName = "localhost",
};
var exchangeName = "topic_logs";

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();


channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);

var queueName = channel.QueueDeclare(queue: "").QueueName;

if (args.Length < 1)
{
    Console.Error.WriteLine("Usage: {0} [binding_key...]",
                            Environment.GetCommandLineArgs()[0]);
    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();
    Environment.ExitCode = 1;
    return;
}

foreach (var bindingKey in args)
{
    channel.QueueBind(queue: queueName,
                      exchange: exchangeName,
                      routingKey: bindingKey);
}
 
Console.WriteLine("Waiting for message");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"[X] Received: {message}, routing key:{ea.RoutingKey}");
};

channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();