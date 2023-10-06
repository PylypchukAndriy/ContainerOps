using ContainerOps.Application.Services;
using ContainerOps.Infrastructure.Options;
using ContainerOps.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ContainerOps.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .Configure<ContainerSettings>(configuration.GetSection(ContainerSettings.Position))
            .Configure<ChannelSettings>(configuration.GetSection(ChannelSettings.Position));

        services
            .AddSingleton<IContainerManager, ContainerManager>()
            .AddSingleton<IContainerMapper, ContainerMapper>()
            .AddSingleton<IContainerCommandQueue, ContainerCommandQueue>();

        return services;
    }
}