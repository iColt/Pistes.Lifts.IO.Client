using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Pistes.Lifts.IO.CloudNotificator
{
    public class CarezzaNotificatorFunction
    {
        private readonly ILogger _logger;

        public CarezzaNotificatorFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CarezzaNotificatorFunction>();
        }

        [Function("CarezzaNotificatorFunction")]
        public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            
            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
    }
}
