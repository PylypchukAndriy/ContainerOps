namespace ContainerOps.Application.Services;

public interface IContainerManager : IDisposable
{
    Task<Guid> CreateAsync(Guid externalId, string image, CancellationToken token = default);

    Task<bool> StartAsync(Guid externalId, CancellationToken token = default);

    Task<bool> StopAsync(Guid externalId, CancellationToken token = default);

    Task<Guid> DeleteAsync(Guid externalId, CancellationToken token = default);
}