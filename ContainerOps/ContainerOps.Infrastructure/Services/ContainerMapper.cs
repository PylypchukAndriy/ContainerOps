using ContainerOps.Application.Services;
using ContainerOps.Domain.Exceptions;
using System.Collections.Concurrent;

namespace ContainerOps.Infrastructure.Services;

internal sealed class ContainerMapper : IContainerMapper
{
    private readonly ConcurrentDictionary<Guid, string> _map = new();

    public string GetInternalId(Guid externalId)
    {
        if (!_map.TryGetValue(externalId, out string? internalId))
        {
            throw new ContainerNotMappedException(externalId);
        }

        return internalId;
    }


    public void MapContainer(Guid externalId, string internalId)
    {
        if (_map.ContainsKey(externalId))
        {
            throw new ContainerAlreadyMappedException(externalId);
        }

        _map[externalId] = internalId;
    }

    public bool UnmapContainer(Guid externalId) => _map.Keys.Remove(externalId);

    public void EnsureContainerUnmapped(Guid externalId)
    {
        if (_map.ContainsKey(externalId))
        {
            throw new ContainerAlreadyMappedException(externalId);
        }
    }
}