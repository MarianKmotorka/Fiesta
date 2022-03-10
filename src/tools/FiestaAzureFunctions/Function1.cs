using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace FiestaAzureFunctions
{
    public static class Function1
    {
        [FunctionName("PreventSleepModeByCallingHealthCheck")]
        public static async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger log)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync("https://fiesta-api.azurewebsites.net/health");
            var content = await response.Content.ReadAsStringAsync();

            log.LogInformation(content);
        }
    }
}
