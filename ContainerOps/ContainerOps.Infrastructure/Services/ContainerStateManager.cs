using ContainerOps.Application.Services;
using ContainerOps.Domain;
using ContainerOps.Domain.Exceptions;
using System.Collections.Concurrent;

namespace ContainerOps.Infrastructure.Services;

internal sealed class ContainerStateManager : IContainerStateManager
{
    private readonly ConcurrentDictionary<Guid, Container> _map = new();

    public string GetContainerInternalId(Guid externalId)
    {
        Container container = GetOrThrowNotFound(externalId);

        return container.InternalId;
    }

    public string GetContainerStatus(Guid externalId)
    {
        Container container = GetOrThrowNotFound(externalId);

        return container.Status.ToString();
    }

    public void AddCreatedContainer(Guid externalId, string internalId)
    {
        EnsureOfContainerAbsence(externalId);

        _map[externalId] = new Container(internalId) { Status = ContainerStatus.Created };
    }

    public void ChangeContainerStatusToStarted(Guid externalId)
    {
        Container container = GetOrThrowNotFound(externalId);
        container.Status = ContainerStatus.Running;
    }

    public void ChangeContainerStatusToStopped(Guid externalId)
    {
        Container container = GetOrThrowNotFound(externalId);
        container.Status = ContainerStatus.Stopped;
    }

    public void DeleteContainer(Guid externalId) => _map.Keys.Remove(externalId);

    public void EnsureOfContainerAbsence(Guid externalId)
    {
        if (_map.ContainsKey(externalId))
        {
            throw new ContainerAlreadyCreatedException(externalId);
        }
    }

    private Container GetOrThrowNotFound(Guid externalId)
    {
        if (!_map.TryGetValue(externalId, out Container? container))
        {
            throw new ContainerNotFoundException(externalId);
        }

        return container;
    }
}