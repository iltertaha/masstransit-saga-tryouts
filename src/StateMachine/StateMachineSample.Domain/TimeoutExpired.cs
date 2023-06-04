using System.Runtime.CompilerServices;
using MassTransit;

namespace StateMachineSample.Events;


public class TimeoutExpired
{
    public Guid ExpirationTokenId { get; set; }
    
    [ModuleInitializer]
    internal static  void Init()
    {
        GlobalTopology.Send.UseCorrelationId<TimeoutExpired>(x=> x.ExpirationTokenId);
    }
}