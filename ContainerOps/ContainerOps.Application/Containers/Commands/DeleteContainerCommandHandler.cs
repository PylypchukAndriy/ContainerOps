using ContainerOps.Application.Services;
using Mediator;

namespace ContainerOps.Application.Containers.Commands;

public readonly record struct DeleteContainerCommand(Guid ExternalId) : IRequest<Guid>;

internal sealed class DeleteContainerCommandHandler : IRequestHandler<DeleteContainerCommand, Guid>
{
    private readonly IContainerManager _containerManager;

    public DeleteContainerCommandHandler(IContainerManager containerManager)
    {
        _containerManager = containerManager;
    }

    public async ValueTask<Guid> Handle(DeleteContainerCommand request, CancellationToken token) =>
        await _containerManager.DeleteAsync(request.ExternalId, token);
}