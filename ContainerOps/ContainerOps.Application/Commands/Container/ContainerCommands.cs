namespace ContainerOps.Application.Commands.Container;

public abstract record ContainerCommand(Guid ExternalId);

public sealed record CreateContainerCommand(Guid ExternalId, string ImageName) : ContainerCommand(ExternalId);

public sealed record StartContainerCommand(Guid ExternalId) : ContainerCommand(ExternalId);

public sealed record StopContainerCommand(Guid ExternalId) : ContainerCommand(ExternalId);

public sealed record DeleteContainerCommand(Guid ExternalId) : ContainerCommand(ExternalId);