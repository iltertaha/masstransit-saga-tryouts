using Hangfire;
using Hangfire.MemoryStorage;
using MassTransit;
using StateMachineSample.API.Consumers;
using StateMachineSample.API.Settings;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var messageBrokerQueueSettings = builder.Configuration.GetSection("MessageBroker:QueueSettings").Get<MessageBrokerQueueSettings>(); 


builder.Services.AddHangfire(h =>
{
    h.UseRecommendedSerializerSettings();
    h.UseMemoryStorage();
});

builder.Services.AddMassTransit(x =>
{
    Uri schedulerEndpoint = new Uri("queue:scheduler");
    
    x.AddMessageScheduler(schedulerEndpoint);
    
    x.UsingRabbitMq((context,cfg) =>
    {
        cfg.Host(messageBrokerQueueSettings.HostName, messageBrokerQueueSettings.VirtualHost, h => {
            h.Username(messageBrokerQueueSettings.UserName);
            h.Password(messageBrokerQueueSettings.Password);
        });

        cfg.UseMessageScheduler(schedulerEndpoint);
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