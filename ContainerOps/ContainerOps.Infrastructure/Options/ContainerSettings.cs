namespace ContainerOps.Infrastructure.Options;

internal class ContainerSettings
{
    public const string Position = "ContainerSettings";

    public uint? WaitBeforeKillSeconds { get; set; }
}