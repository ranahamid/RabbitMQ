﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

Console.WriteLine("Hello, World!");
var factory = new ConnectionFactory()
{
    HostName = "localhost",
};

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare
    (
    queue: "hello",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null
    );
Console.WriteLine("Waiting for message");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body= ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"[X] Received: {message}");
};

channel.BasicConsume(queue:"hello", autoAck:true, consumer:consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();