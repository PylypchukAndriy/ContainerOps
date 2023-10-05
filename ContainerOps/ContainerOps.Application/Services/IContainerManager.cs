namespace ContainerOps.Application.Services;

public interface IContainerManager : IDisposable
{
    Task<string> CreateAsync(string image, CancellationToken token = default);

    Task<bool> StartAsync(string id, CancellationToken token = default);

    Task<bool> StopAsync(string id, CancellationToken token = default);

    Task DeleteAsync(string id, CancellationToken token = default);
}