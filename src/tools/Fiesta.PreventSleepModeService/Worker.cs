using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fiesta.PreventWebApiFromSleepModeService
{
    public class Worker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<Worker> _logger;
        private readonly TimeSpan _pollingPeriod;
        private readonly string _apiBaseUrl;

        public Worker(IServiceProvider serviceProvider, ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _apiBaseUrl = configuration.GetValue<string>("ApiBaseUrl");
            _pollingPeriod = configuration.GetValue<TimeSpan>("PollingPeriod");
        }

        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await Task.Yield();

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("Worker loop triggered.");

                    try
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var client = scope.ServiceProvider.GetRequiredService<HttpClient>();
                            await MakeApiCall(client, cancellationToken);
                        }

                        _logger.LogDebug("Worker loop delayed for {delay}.", _pollingPeriod);
                    }
                    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                    {
                        // Pass
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occured during Worker execution.");
                    }

                    await Task.Delay(_pollingPeriod, cancellationToken);
                }
            }
            finally
            {
                _logger.LogDebug("Worker stopped.");
            }
        }

        private async Task MakeApiCall(HttpClient client, CancellationToken cancellationToken)
        {
            var url = _apiBaseUrl + "/health";
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"RESPONSE: {content}");
        }
    }
}
