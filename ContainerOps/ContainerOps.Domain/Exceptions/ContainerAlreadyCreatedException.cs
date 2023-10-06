namespace ContainerOps.Domain.Exceptions;

public class ContainerAlreadyCreatedException : Exception
{
    public ContainerAlreadyCreatedException(Guid externalId) : this(externalId, null) { }

    public ContainerAlreadyCreatedException(Guid externalId, string? message) : base(message)
    {
        ExternalId = externalId;
    }

    public Guid ExternalId { get; }
}