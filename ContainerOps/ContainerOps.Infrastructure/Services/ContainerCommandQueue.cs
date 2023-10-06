using ContainerOps.Application.Commands.Container;
using ContainerOps.Application.Services;
using ContainerOps.Infrastructure.Options;
using Microsoft.Extensions.Options;
using System.Threading.Channels;

namespace ContainerOps.Infrastructure.Services;

internal sealed class ContainerCommandQueue : IContainerCommandQueue
{
    private readonly Channel<ContainerCommand> _channel;

    public ContainerCommandQueue(IOptions<ChannelSettings> options)
    {
        ChannelSettings settings = options.Value;
        _channel = Channel.CreateBounded<ContainerCommand>(new BoundedChannelOptions(settings.Capacity)
        {
            AllowSynchronousContinuations = settings.AllowSynchronousContinuations,
            FullMode = settings.FullMode,
            SingleReader = settings.SingleReader,
            SingleWriter = settings.SingleWriter
        });

    }

    public ValueTask Enqueue(ContainerCommand command, CancellationToken token = default) =>
        _channel.Writer.WriteAsync(command, token);

    public IAsyncEnumerable<ContainerCommand> DequeueAll(CancellationToken token = default) =>
         _channel.Reader.ReadAllAsync(token);
}