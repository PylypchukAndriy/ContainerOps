namespace ContainerOps.Application.Services;

public interface IContainerMapper
{
    string GetInternalId(Guid externalId);

    void MapContainer(Guid externalId, string internalId);

    bool UnmapContainer(Guid externalId);

    void EnsureContainerUnmapped(Guid externalId);
}