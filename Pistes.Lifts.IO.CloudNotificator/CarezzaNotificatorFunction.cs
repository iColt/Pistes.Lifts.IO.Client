using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Pistes.Lifts.IO.CloudNotificator
{
    public class CarezzaNotificatorFunction
    {
        private readonly ILogger<CarezzaNotificatorFunction> _logger;

        //create bot
        public CarezzaNotificatorFunction(ILogger<CarezzaNotificatorFunction> logger)
        {
            _logger = logger;
        }

        //check bot created
        [Function("CarezzaNotificatorFunctionTrigger")]
        public void TriggerImpl([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }

        [Function("CarezzaNotificatorFunction")]
        public HttpResponseData FunctionImpl([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");

            return response;
        }

        //function to create bot or check, it created already
        //use container
        //var value = Environment.GetEnvironmentVariable("BOT_TOKEN")
        ///var value = Environment.GetEnvironmentVariable("WEBHOOK_ADDRESS")
    }
}
