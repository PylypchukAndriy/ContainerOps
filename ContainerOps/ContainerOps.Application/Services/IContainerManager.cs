namespace ContainerOps.Application.Services;

public interface IContainerManager : IDisposable
{
    Task CreateAsync(Guid externalId, string image, CancellationToken token = default);

    Task StartAsync(Guid externalId, CancellationToken token = default);

    Task StopAsync(Guid externalId, CancellationToken token = default);

    Task DeleteAsync(Guid externalId, CancellationToken token = default);
}