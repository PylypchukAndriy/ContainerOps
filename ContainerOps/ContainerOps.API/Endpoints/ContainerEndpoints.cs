using ContainerOps.Application.Commands.Container;
using ContainerOps.Application.Services;
using ContainerOps.Domain.Exceptions;

namespace ContainerOps.API.Endpoints;

internal static class ContainerEndpoints
{
    public static void MapContainerEndpoints(this IEndpointRouteBuilder app)
    {
        const string Route = "api/containers";

        app.MapGet(Route + "/{containerId}", (Guid containerId, IContainerStateManager manager) =>
        {
            try
            {
                string status = manager.GetContainerStatus(containerId);

                return Results.Ok(status);
            }
            catch (ContainerNotFoundException)
            {
                return Results.NotFound();
            }
        });

        app.MapPost($"{Route}/create", async (string imageName, CancellationToken token, IContainerCommandQueue queue) =>
        {
            var externalId = Guid.NewGuid();

            await queue.Enqueue(new CreateContainerCommand(externalId, imageName), token);

            return Results.Created($"{Route}/{externalId}", externalId);
        });

        app.MapPost($"{Route}/start", async (Guid containerId, CancellationToken token, IContainerCommandQueue queue) =>
        {
            await queue.Enqueue(new StartContainerCommand(containerId), token);

            return Results.NoContent();
        });

        app.MapPost($"{Route}/stop", async (Guid containerId, CancellationToken token, IContainerCommandQueue queue) =>
        {
            await queue.Enqueue(new StopContainerCommand(containerId), token);

            return Results.NoContent();
        });

        app.MapDelete(Route, async (Guid containerId, CancellationToken token, IContainerCommandQueue queue) =>
        {
            await queue.Enqueue(new DeleteContainerCommand(containerId), token);

            return Results.NoContent();
        });
    }
}