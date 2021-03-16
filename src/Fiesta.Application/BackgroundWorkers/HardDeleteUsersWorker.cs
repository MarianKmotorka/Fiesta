using System;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Options;
using Fiesta.Application.Features.Users;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fiesta.Application.BackgroundWorkers
{
    public class HardDeleteUsersWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<HardDeleteUsersWorker> _logger;
        private readonly HardDeleteUsersWorkerOptions _options;

        public HardDeleteUsersWorker(IServiceProvider serviceProvider, ILogger<HardDeleteUsersWorker> logger, HardDeleteUsersWorkerOptions options)
        {
            _logger = logger;
            _options = options;
            _serviceProvider = serviceProvider;
        }

        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (!_options.Enabled)
            {
                _logger.LogDebug("Hard Delete Users Worker disabled by configuration.");
                return;
            }

            _logger.LogDebug("Started Hard Delete Users Worker.");
            await Task.Yield();

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("Hard Delete Users Worker loop triggered.");

                    try
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                            await mediator.Send(new HardDeleteUsers.Command(), cancellationToken);
                        }

                        _logger.LogDebug("Hard Delete Users Worker loop delayed for {delay}.", _options.PollingPeriod);
                    }
                    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                    {
                        // Pass
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occured during Hard Delete Users Worker execution.");
                    }

                    await Task.Delay(_options.PollingPeriod, cancellationToken);
                }
            }
            finally
            {
                _logger.LogDebug("Hard Delete Users Worker stopped.");
            }
        }
    }
}
