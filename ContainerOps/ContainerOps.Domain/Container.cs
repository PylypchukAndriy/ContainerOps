namespace ContainerOps.Domain;

public sealed record Container(string InternalId)
{
    public ContainerStatus Status { get; set; }
}