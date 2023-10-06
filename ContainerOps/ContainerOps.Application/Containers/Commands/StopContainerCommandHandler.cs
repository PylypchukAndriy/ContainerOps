using ContainerOps.Application.Services;
using Mediator;

namespace ContainerOps.Application.Containers.Commands;

public readonly record struct StopContainerCommand(Guid ExternalId) : IRequest<bool>;

internal sealed class StopContainerCommandHandler : IRequestHandler<StopContainerCommand, bool>
{
    private readonly IContainerManager _containerManager;

    public StopContainerCommandHandler(IContainerManager containerManager)
    {
        _containerManager = containerManager;
    }

    public async ValueTask<bool> Handle(StopContainerCommand request, CancellationToken token) =>
        await _containerManager.StopAsync(request.ExternalId, token);

}