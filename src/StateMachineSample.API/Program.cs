using MassTransit;
using StateMachineSample.API.Consumers;
using StateMachineSample.API.Settings;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var messageBrokerQueueSettings = builder.Configuration.GetSection("MessageBroker:QueueSettings").Get<MessageBrokerQueueSettings>(); 

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context,cfg) =>
    {
        cfg.Host(messageBrokerQueueSettings.HostName, messageBrokerQueueSettings.VirtualHost, h => {
            h.Username(messageBrokerQueueSettings.UserName);
            h.Password(messageBrokerQueueSettings.Password);
        });

        cfg.ConfigureEndpoints(context);
    });

    x.AddConsumer<OrderProcessInitializationEventConsumer>();
    x.AddRequestClient<OrderProcessInitializationEventConsumer>();
    
    

});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();