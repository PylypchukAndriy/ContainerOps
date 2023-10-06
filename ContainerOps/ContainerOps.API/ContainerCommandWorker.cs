using ContainerOps.Application.Commands.Container;
using ContainerOps.Application.Services;

namespace ContainerOps.API
{
    public class ContainerCommandWorker : BackgroundService
    {
        private readonly ILogger<ContainerCommandWorker> _logger;
        private readonly IContainerCommandQueue _queue;
        private readonly IContainerManager _containerManager;

        public ContainerCommandWorker(
            ILogger<ContainerCommandWorker> logger,
            IContainerCommandQueue queue,
            IContainerManager containerManager)
        {
            _logger = logger;
            _queue = queue;
            _containerManager = containerManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("{BackgroundServiceName} has started", nameof(ContainerCommandWorker));

            await foreach (ContainerCommand command in _queue.DequeueAll(stoppingToken))
            {
                try
                {
                    switch (command)
                    {
                        case CreateContainerCommand ccc:
                            await _containerManager.CreateAsync(ccc.ExternalId, ccc.ImageName, stoppingToken);
                            break;
                        case StartContainerCommand scc:
                            await _containerManager.StartAsync(scc.ExternalId, stoppingToken);
                            break;
                        case StopContainerCommand scc:
                            await _containerManager.StopAsync(scc.ExternalId, stoppingToken);
                            break;
                        case DeleteContainerCommand dcc:
                            await _containerManager.DeleteAsync(dcc.ExternalId, stoppingToken);
                            break;

                        default: throw new NotSupportedException($"{command.GetType()} is not supported");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occured executing command {Command}", command);
                }
            }
        }
    }
}