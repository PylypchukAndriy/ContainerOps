using ContainerOps.Application.Services;
using Mediator;

namespace ContainerOps.Application.Containers.Commands;

public readonly record struct StartContainerCommand(Guid ExternalId) : IRequest<bool>;

internal sealed class StartContainerCommandHandler : IRequestHandler<StartContainerCommand, bool>
{
    private readonly IContainerManager _containerManager;

    public StartContainerCommandHandler(IContainerManager containerManager)
    {
        _containerManager = containerManager;
    }

    public async ValueTask<bool> Handle(StartContainerCommand request, CancellationToken token) =>
        await _containerManager.StartAsync(request.ExternalId, token);
}