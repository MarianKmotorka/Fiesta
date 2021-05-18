using System;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Options;
using Fiesta.Application.Features.Notifications;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fiesta.Application.BackgroundWorkers
{
    public class NotificationDeletionWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationDeletionWorker> _logger;
        private readonly NotificationDeletionWorkerOptions _options;

        public NotificationDeletionWorker(IServiceProvider serviceProvider, ILogger<NotificationDeletionWorker> logger, NotificationDeletionWorkerOptions options)
        {
            _logger = logger;
            _options = options;
            _serviceProvider = serviceProvider;
        }

        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (!_options.Enabled)
            {
                _logger.LogDebug("Notification Deletion Worker disabled by configuration.");
                return;
            }

            _logger.LogDebug("Started Notification Deletion Worker.");
            await Task.Yield();

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("Notification Deletion Worker loop triggered.");

                    try
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                            var command = new DeleteOldNotifications.Command
                            {
                                BatchSize = _options.BatchSize,
                                DeleteAfter = _options.DeleteAfter
                            };
                            await mediator.Send(command, cancellationToken);
                        }

                        _logger.LogDebug("Notification Deletion Worker loop delayed for {delay}.", _options.PollingPeriod);
                    }
                    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                    {
                        // Pass
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occured during Notification Deletion Worker execution.");
                    }

                    await Task.Delay(_options.PollingPeriod, cancellationToken);
                }
            }
            finally
            {
                _logger.LogDebug("Notification Deletion Worker stopped.");
            }
        }
    }
}
