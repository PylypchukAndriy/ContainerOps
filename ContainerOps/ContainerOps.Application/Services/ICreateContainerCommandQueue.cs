namespace ContainerOps.Application.Services;

public interface ICreateContainerCommandQueue
{
    ValueTask<Guid> Enqueue(string image, CancellationToken token = default);

    ValueTask<(Guid ExternalId, string Image)> Dequeue(CancellationToken token = default);
}