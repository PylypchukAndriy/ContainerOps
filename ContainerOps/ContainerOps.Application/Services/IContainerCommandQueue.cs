using ContainerOps.Application.Commands.Container;

namespace ContainerOps.Application.Services;

public interface IContainerCommandQueue
{
    ValueTask Enqueue(ContainerCommand command, CancellationToken token = default);

    IAsyncEnumerable<ContainerCommand> DequeueAll(CancellationToken token = default);
}