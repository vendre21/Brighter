using System.Threading;
using System.Threading.Tasks;
using Paramore.Brighter.Eventsourcing.Attributes;
using Paramore.Brighter.Tests.TestDoubles;

namespace Paramore.Brighter.Tests.EventSourcing.TestDoubles
{
    internal class MyStoredCommandHandlerAsync : RequestHandlerAsync<MyCommand> 
    {
        [UseCommandSourcingAsync(step: 1, timing: HandlerTiming.Before)]
        public override async Task<MyCommand> HandleAsync(MyCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await base.HandleAsync(command, cancellationToken).ConfigureAwait(ContinueOnCapturedContext);
        }
    }
}