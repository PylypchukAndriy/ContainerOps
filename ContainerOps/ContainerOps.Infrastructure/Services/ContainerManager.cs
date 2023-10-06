using ContainerOps.Application.Services;
using ContainerOps.Infrastructure.Options;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Options;
using System.Runtime.InteropServices;

namespace ContainerOps.Infrastructure.Services;

internal sealed class ContainerManager : IContainerManager
{
    private readonly DockerClient _client;
    private readonly IOptionsMonitor<ContainerSettings> _containerSettings;
    private readonly IContainerStateManager _containerStateManager;

    public ContainerManager(
        IOptionsMonitor<ContainerSettings> containerSettings,
        IContainerStateManager containerStateManager)
    {
        _containerSettings = containerSettings;
        _containerStateManager = containerStateManager;

        _client = new DockerClientConfiguration(new Uri(GetDockerApiUri())).CreateClient();
    }

    public async Task CreateAsync(Guid externalId, string image, CancellationToken token = default)
    {
        EnsureOfContainerAbsence(externalId);

        AppendTagIfMissing(ref image);

        await PullImageIfAbsentAsync(image, token);

        CreateContainerResponse result = await _client.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Image = image,
            HostConfig = new HostConfig
            {
                DNS = new[] { "8.8.8.8", "8.8.4.4" }
            }
        }, token);

        _containerStateManager.AddCreatedContainer(externalId, result.ID);
    }

    public async Task StartAsync(Guid externalId, CancellationToken token = default)
    {
        bool started = await _client.Containers.StartContainerAsync(GetInternalId(externalId), null, token);
        if (started)
        {
            _containerStateManager.ChangeContainerStatusToStarted(externalId);
        }
    }

    public async Task StopAsync(Guid externalId, CancellationToken token = default)
    {
        bool stopped = await _client.Containers.StopContainerAsync(
                GetInternalId(externalId),
                new ContainerStopParameters
                {
                    WaitBeforeKillSeconds = _containerSettings.CurrentValue.WaitBeforeKillSeconds,
                },
                token);

        if (stopped)
        {
            _containerStateManager.ChangeContainerStatusToStopped(externalId);
        }
    }

    public async Task DeleteAsync(Guid externalId, CancellationToken token = default)
    {
        await _client.Containers.RemoveContainerAsync(
            GetInternalId(externalId),
            new ContainerRemoveParameters
            {
                Force = true,
                RemoveLinks = false,
                RemoveVolumes = true
            },
            token);

        _containerStateManager.DeleteContainer(externalId);
    }

    public void Dispose() => _client.Dispose();

    private void EnsureOfContainerAbsence(Guid externalId) =>
        _containerStateManager.EnsureOfContainerAbsence(externalId);

    private static void AppendTagIfMissing(ref string image) =>
        image = image.Contains(':') ? image : $"{image}:latest";

    private async Task PullImageIfAbsentAsync(string image, CancellationToken token = default)
    {
        await _client.Images.CreateImageAsync(
            new ImagesCreateParameters
            {
                FromImage = image
            },
            null,
            new Progress<JSONMessage>(),
            token);
    }

    private string GetInternalId(Guid externalId) =>
        _containerStateManager.GetContainerInternalId(externalId);

    private static string GetDockerApiUri()
    {
        bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        if (isWindows)
        {
            return "npipe://./pipe/docker_engine";
        }

        bool isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        if (isLinux)
        {
            return "unix:/var/run/docker.sock";
        }

        throw new NotSupportedException("The current operation system is not supported");
    }
}