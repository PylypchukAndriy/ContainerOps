namespace ContainerOps.Application.Services;

public interface ICreateContainerCommandQueue
{
    ValueTask<Guid> Enqueue(string image, CancellationToken token = default);

    IAsyncEnumerable<(Guid ExternalId, string Image)> DequeueAll(CancellationToken token = default);
}