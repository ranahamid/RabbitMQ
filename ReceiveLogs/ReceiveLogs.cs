using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

Console.WriteLine("Hello, World!");
//var qName = "My_Tasks";
var exchangeName = "logs";
var factory = new ConnectionFactory()
{
    HostName = "localhost",
};

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout);

var queueName = channel.QueueDeclare().QueueName;
channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: "");

//channel.QueueDeclare
//    (
//    queue: qName,
//    durable: true, //durable
//    exclusive: false,
//    autoDelete: false,
//    arguments: null
//    );
//channel.BasicQos(
//    prefetchSize: 0,
//    prefetchCount: 1,
//    global: false
//    );
Console.WriteLine("Waiting for message");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"[X] Received: {message}");

    int dots = message.Split('.').Length - 1;
    Thread.Sleep(dots * 1000);
    Console.WriteLine("[X] done");

    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
};

channel.BasicConsume(
    queue: queueName,
    autoAck: true,
    consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();

