using Carter;
using Microsoft.EntityFrameworkCore;
using Orders.Api.Features.CreateOrder;
using Orders.Api.Services;
using Orders.Contracts;
using Orders.Database;
using Swashbuckle.AspNetCore.Filters;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

var dbConnectionString = builder.Configuration.GetConnectionString("OrdersDb");
builder.Services.AddOrdersDbContext(dbConnectionString);
builder.Services.AddHostedService<DbInitializer>();
builder.Services.AddTransient<OrderProcessor>();

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
    
    options.PersistMessagesWithPostgresql(dbConnectionString, schemaName: "wolverine");
    options.UseEntityFrameworkCoreTransactions();
    options.Policies.UseDurableLocalQueues();

    options.PublishMessage<CreateOrderMessage>().ToRabbitQueue(QueueNames.Orders);
    options.ListenToRabbitQueue(QueueNames.Events);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.ExampleFilters();
});
builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();
builder.Services.AddCarter();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapCarter();

app.Run();