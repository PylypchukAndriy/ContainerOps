using ContainerOps.Application.Services;
using Mediator;

namespace ContainerOps.Application.Containers.Commands;

public readonly record struct CreateContainerCommand(string ImageName) : IRequest<Guid>;

internal sealed class CreateContainerCommandHandler : IRequestHandler<CreateContainerCommand, Guid>
{
    private readonly ICreateContainerCommandQueue _queue;

    public CreateContainerCommandHandler(ICreateContainerCommandQueue queue)
    {
        _queue = queue;
    }

    public ValueTask<Guid> Handle(CreateContainerCommand request, CancellationToken token) =>
        _queue.Enqueue(request.ImageName, token);
}