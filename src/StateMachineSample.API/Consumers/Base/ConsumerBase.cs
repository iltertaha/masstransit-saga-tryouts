using MassTransit;

namespace StateMachineSample.API.Consumers.Base;

public abstract class ConsumerBase<T> : IConsumer<T> where T : class
{
    public async Task Consume(ConsumeContext<T> context)
    {
        await Task.Delay(TimeSpan.FromSeconds(5));
        await ConsumeInternal(context);
    }

    protected abstract Task ConsumeInternal(ConsumeContext<T> context);
}