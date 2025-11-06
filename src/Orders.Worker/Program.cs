using JasperFx.Core;
using Microsoft.EntityFrameworkCore;
using Orders.Contracts;
using Orders.Database;
using Orders.Worker.Consumers;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.ErrorHandling;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;

var builder = Host.CreateApplicationBuilder(args);

var dbConnectionString = builder.Configuration.GetConnectionString("OrdersDb");
builder.Services.AddOrdersDbContext(dbConnectionString);

builder.Services.AddTransient<DomainValidator>();
builder.Services.AddSingleton<CustomerApiService>();

builder.Services.AddDbContextWithWolverineIntegration<OrdersDbContext>(optionsBuilder =>
{
    optionsBuilder.UseNpgsql(dbConnectionString);
}, "wolverine");

builder.UseWolverine(options =>
{
    var hostName = builder.Configuration["RabbitMq:HostName"];
    var port = builder.Configuration["RabbitMq:Port"];
    var userName = builder.Configuration["RabbitMq:UserName"];
    var password = builder.Configuration["RabbitMq:Password"];
    var rabbitMqConnectionString = $"amqp://{userName}:{password}@{hostName}:{port}";
    options.UseRabbitMq(rabbitMqConnectionString).AutoProvision();
    
    options.ListenToRabbitQueue(QueueNames.Orders);
    
    options.PublishMessage<OrderCreatedEvent>().ToRabbitQueue(QueueNames.Events);
    
    options.PersistMessagesWithPostgresql(dbConnectionString, schemaName: "wolverine");
    
    options.UseEntityFrameworkCoreTransactions();
    
    options.Policies.UseDurableLocalQueues();

    options.OnException<DomainValidationException>().MoveToErrorQueue().And(async (_, context, ex) =>
    {
        if (context.Envelope?.Message is CreateOrderMessage msg)
        {
            await context.RespondToSenderAsync(new OrderRejectedMessage { OrderId = msg.OrderId, Reason = ex.Message });
        }
    });
});

var host = builder.Build();
host.Run();