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

    public ContainerManager(IOptionsMonitor<ContainerSettings> containerSettings)
    {
        _containerSettings = containerSettings;

        _client = new DockerClientConfiguration(new Uri(GetDockerApiUri())).CreateClient();
    }

    public async Task<string> CreateAsync(string image, CancellationToken token = default)
    {
        AppendTagIfMissing(ref image);

        await EnsureImageIsPulledAsync(image, token);

        CreateContainerResponse result = await _client.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Image = image,
            HostConfig = new HostConfig
            {
                DNS = new[] { "8.8.8.8", "8.8.4.4" }
            }
        }, token);

        return result.ID;
    }

    public async Task<bool> StartAsync(string id, CancellationToken token = default)
    {
        return await _client.Containers.StartContainerAsync(id, null, token);
    }

    public async Task<bool> StopAsync(string id, CancellationToken token = default)
    {
        return await _client.Containers.StopContainerAsync(
            id,
            new ContainerStopParameters
            {
                WaitBeforeKillSeconds = _containerSettings.CurrentValue.WaitBeforeKillSeconds,
            },
            token);
    }

    public async Task DeleteAsync(string id, CancellationToken token = default)
    {
        await _client.Containers.RemoveContainerAsync(
            id,
            new ContainerRemoveParameters
            {
                Force = true,
                RemoveLinks = false,
                RemoveVolumes = true
            },
            token);
    }

    public void Dispose() => _client.Dispose();

    private static void AppendTagIfMissing(ref string image) =>
        image = image.Contains(':') ? image : $"{image}:latest";

    private async Task EnsureImageIsPulledAsync(string image, CancellationToken token = default)
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