namespace ContainerOps.Domain.Exceptions;

public class ContainerNotFoundException : Exception
{
    public ContainerNotFoundException(Guid externalId) : this(externalId, null) { }

    public ContainerNotFoundException(Guid externalId, string? message) : base(message)
    {
        ExternalId = externalId;
    }

    public Guid ExternalId { get; }
}