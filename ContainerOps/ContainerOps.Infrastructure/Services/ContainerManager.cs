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
    private readonly IContainerMapper _containerMapper;

    public ContainerManager(IOptionsMonitor<ContainerSettings> containerSettings, IContainerMapper containerMapper)
    {
        _containerSettings = containerSettings;
        _containerMapper = containerMapper;

        _client = new DockerClientConfiguration(new Uri(GetDockerApiUri())).CreateClient();
    }

    public async Task<Guid> CreateAsync(Guid externalId, string image, CancellationToken token = default)
    {
        EnsureContainerUnmapped(externalId);

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

        _containerMapper.MapContainer(externalId, result.ID);

        return externalId;
    }

    public async Task<bool> StartAsync(Guid externalId, CancellationToken token = default)
    {
        return await _client.Containers.StartContainerAsync(GetInternalId(externalId), null, token);
    }

    public async Task<bool> StopAsync(Guid externalId, CancellationToken token = default)
    {
        return await _client.Containers.StopContainerAsync(
            GetInternalId(externalId),
            new ContainerStopParameters
            {
                WaitBeforeKillSeconds = _containerSettings.CurrentValue.WaitBeforeKillSeconds,
            },
            token);
    }

    public async Task<Guid> DeleteAsync(Guid externalId, CancellationToken token = default)
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

        _containerMapper.UnmapContainer(externalId);

        return externalId;
    }

    public void Dispose() => _client.Dispose();

    private void EnsureContainerUnmapped(Guid externalId) => _containerMapper.EnsureContainerUnmapped(externalId);

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

    private string GetInternalId(Guid externalId) => _containerMapper.GetInternalId(externalId);

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