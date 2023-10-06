using ContainerOps.Application.Services;
using ContainerOps.Infrastructure.Options;
using Microsoft.Extensions.Options;
using System.Threading.Channels;

namespace ContainerOps.Infrastructure.Services;

internal sealed class CreateContainerCommandQueue : ICreateContainerCommandQueue
{
    private readonly Channel<(Guid ExternalId, string Image)> _channel;

    public CreateContainerCommandQueue(IOptions<ChannelSettings> options)
    {
        ChannelSettings settings = options.Value;
        _channel = Channel.CreateBounded<(Guid, string)>(new BoundedChannelOptions(settings.Capacity)
        {
            AllowSynchronousContinuations = settings.AllowSynchronousContinuations,
            FullMode = settings.FullMode,
            SingleReader = settings.SingleReader,
            SingleWriter = settings.SingleWriter
        });

    }

    public async ValueTask<Guid> Enqueue(string image, CancellationToken token = default)
    {
        Guid externalId = Guid.NewGuid();

        await _channel.Writer.WriteAsync((externalId, image), token);

        return externalId;
    }

    public ValueTask<(Guid ExternalId, string Image)> Dequeue(CancellationToken token = default) =>
        _channel.Reader.ReadAsync(token);
}