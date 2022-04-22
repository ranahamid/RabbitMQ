
using RabbitMQ.Client;
using System.Text;

Console.WriteLine("Hello, World!");
var qName = "My_Tasks";
var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: qName, durable: true, exclusive: false, autoDelete: false, arguments: null);

var list = new string[5];
list[0] = "Hello ll..";
list[1] = "Hello kk.00.";
list[2] = "Hello oo..";
list[3] = "Hello TT..";
list[4] = "Hello PP..";
string message = GetMessage(list);

var body = Encoding.UTF8.GetBytes(message);

var properties= channel.CreateBasicProperties();
properties.Persistent = true;

channel.BasicPublish(
    exchange: "",
    routingKey: qName,
    basicProperties: properties,
    body: body
    );
Console.WriteLine("[X] sebt {0}", message);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();

static string GetMessage(string[] args)
{
    var message = "Hello Eorld";
    if (args.Length > 0)
    {
        message = string.Join("U+002CU+0020", args);
    }
    return message;
}