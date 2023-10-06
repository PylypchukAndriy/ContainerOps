namespace ContainerOps.Domain.Exceptions;

public class ContainerNotMappedException : Exception
{
    public ContainerNotMappedException(Guid externalId) : this(externalId, null) { }

    public ContainerNotMappedException(Guid externalId, string? message) : base(message)
    {
        ExternalId = externalId;
    }

    public Guid ExternalId { get; }
}