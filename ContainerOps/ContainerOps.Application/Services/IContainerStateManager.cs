namespace ContainerOps.Application.Services;

public interface IContainerStateManager
{
    string GetContainerInternalId(Guid externalId);

    string GetContainerStatus(Guid externalId);

    void AddCreatedContainer(Guid externalId, string internalId);

    void ChangeContainerStatusToStarted(Guid externalId);

    void ChangeContainerStatusToStopped(Guid externalId);

    void DeleteContainer(Guid externalId);

    void EnsureOfContainerAbsence(Guid externalId);
}