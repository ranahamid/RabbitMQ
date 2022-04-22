//dotnet run "kern.*" "*.critical"

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;

public class RpcClient
{
    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly string replyQueueName;
    private readonly EventingBasicConsumer consumer;
    private readonly BlockingCollection<string> respQueue = new BlockingCollection<string>();
    private readonly IBasicProperties props;
    public string  queueName = "rpc_queue";
    public RpcClient()
    {
       

        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
        };
        var exchangeName = "topic_logs";

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        var replyQueueName = channel.QueueDeclare(queue: "").QueueName; 

        var consumer = new EventingBasicConsumer(channel);

        var props = channel.CreateBasicProperties();      
   var correlationId=Guid.NewGuid().ToString();
        props.CorrelationId =
  props.ReplyTo = replyQueueName;

        
       
         
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            if(ea.BasicProperties.CorrelationId== correlationId)
            {
                respQueue.Add(message);
            }
            Console.WriteLine($"[X] Received: {message}, routing key:{ea.RoutingKey}");
        };

        channel.BasicConsume(queue: replyQueueName, autoAck: true, consumer: consumer);

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

    public string Call(string message)
    {
        var resBytes = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(
            exchange: "",
            routingKey: queueName,
            basicProperties: props,
            body: resBytes
            );
     return respQueue.Take();
    }

    public void Close()
    {
        connection.Close();
    }
}
public class Rpc
{
    public static void Main()
    {
        var rpcClient = new RpcClient();
        var response = rpcClient.Call("30");
        Console.WriteLine($"[.] Got {response}");
        rpcClient.Close();
    }
}

 