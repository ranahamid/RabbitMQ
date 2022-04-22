
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

 var queueName = "rpc_queue";
var exchangeName = "topic_logs";

var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);



var consumer = new EventingBasicConsumer(channel);
channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

Console.WriteLine("[X] Waiting RPC request.");

consumer.Received += (model, ea) =>
{
    string response = String.Empty; 
    var body = ea.Body.ToArray();
    var props = ea.BasicProperties;
    var replayProps = channel.CreateBasicProperties();
    replayProps.CorrelationId = props.CorrelationId;

    try
    {
        var message = Encoding.UTF8.GetString(body);
        int n= int.Parse(message);
        Console.WriteLine($"[X] Received: {message}, routing key:{ea.RoutingKey}");
        response = fib(n).ToString();

    }
    catch (Exception ex)
    {
        response = "";
    }
    finally
    {
        var resBytes = Encoding.UTF8.GetBytes(response);
        channel.BasicPublish(
            exchange: "",
            routingKey: props.ReplyTo,
            basicProperties: replayProps,
            body: resBytes
            );
        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
    }
};


 


 

//var properties = channel.CreateBasicProperties();
//properties.Persistent = true;
 

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();

static int fib(int n)
{
    if (n == 0 || n == 1)
        return n;
    return fib(n-1)+ fib(n - 2);
}