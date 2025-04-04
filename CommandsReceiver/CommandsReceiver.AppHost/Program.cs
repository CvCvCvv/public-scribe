var builder = DistributedApplication.CreateBuilder(args);

var rabbitmq = builder.AddRabbitMQ("rabbitmq");

builder.AddProject<Projects.CommandsReceiver>("commandsreceiver")
    .WithReference(rabbitmq);

builder.Build().Run();
