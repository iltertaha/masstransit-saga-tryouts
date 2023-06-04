using MassTransit;
using StateMachineSample.Events;

namespace StateMachineSample.StateMachine;

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public OrderStateMachine()
    {
        #region EventsDefinitions

        Event(() => OrderProcessInitializationEvent);
        
        Schedule(() => ExpirationSchedule, x => x.ExpirationTokenId, x =>
        {
            x.Delay = TimeSpan.FromSeconds(2);
        });
        
        #endregion


        InstanceState(x => x.CurrentState);

        #region Flow

        During(Initial,
            When(OrderProcessInitializationEvent)
                .Then(x => x.Saga.ExpirationTokenId = new Guid())
                .Schedule(ExpirationSchedule, context => context.Init<TimeoutExpired>(new { context.Message.OrderId }))
                .Then(x => x.Saga.OrderStartDate = DateTime.Now)
                .TransitionTo(OrderProcessInitializedState));
        
        DuringAny(When(TimeoutExpired)
            .Then(_ => Console.WriteLine("timeout event fired"))
            .Unschedule(ExpirationSchedule)
            .TransitionTo(Cancelled));
        
        #endregion


        
    }

    #region Events

    public Event<OrderProcessInitializationEvent> OrderProcessInitializationEvent { get; }
    
    public Schedule<OrderState, TimeoutExpired> ExpirationSchedule { get; }
    
    public Event<TimeoutExpired> TimeoutExpired { get; }

    #endregion


    #region States

    
    public State OrderProcessInitializedState { get; }
    
    public State Cancelled { get; }

    #endregion
}