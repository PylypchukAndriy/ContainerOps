using System.Threading.Channels;

namespace ContainerOps.Infrastructure.Options;

internal class ChannelSettings
{
    public const string Position = "ChannelSettings";

    public bool AllowSynchronousContinuations { get; set; }

    public int Capacity { get; set; }

    public BoundedChannelFullMode FullMode { get; set; }

    public bool SingleWriter { get; set; }

    public bool SingleReader { get; set; }
}