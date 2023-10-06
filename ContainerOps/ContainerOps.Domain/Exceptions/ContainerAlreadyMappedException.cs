namespace ContainerOps.Domain.Exceptions;

public class ContainerAlreadyMappedException : Exception
{
    public ContainerAlreadyMappedException(Guid externalId) : this(externalId, null) { }

    public ContainerAlreadyMappedException(Guid externalId, string? message) : base(message)
    {
        ExternalId = externalId;
    }

    public Guid ExternalId { get; }
}