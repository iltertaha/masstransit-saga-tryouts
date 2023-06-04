using MassTransit;
using Microsoft.AspNetCore.Mvc;
using StateMachineSample.API.Requests;
using StateMachineSample.Events;
using StateMachineSample.Events.Responses;

namespace StateMachineSample.API.Controllers;

public class EventsController
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IRequestClient<OrderProcessInitializationEvent> _orderProcessInitializationEventRequestClient;
    
        
    public EventsController(
        IPublishEndpoint publishEndpoint,
        IRequestClient<OrderProcessInitializationEvent> orderProcessInitializationEventRequestClient)
    {
        _publishEndpoint = publishEndpoint;
        _orderProcessInitializationEventRequestClient = orderProcessInitializationEventRequestClient;
        
    }

    [HttpPost("initialize/order")]
    public async Task<IActionResult> OrderProcessInitializedEvent([FromBody] EventCommonRequest request)
    {
        if (!request.IsRequestResponsePattern)
        {
            await _publishEndpoint.Publish<OrderProcessInitializationEvent>(new {request.OrderId});
            return new NoContentResult();
        }

        var result = await _orderProcessInitializationEventRequestClient.GetResponse<OrderProcessInitiazationDto>(new {request.OrderId});

        return new NoContentResult();
    }
    
}