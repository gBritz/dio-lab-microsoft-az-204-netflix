using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace fnPostDatabase
{
    public class PostDatabase
    {
        private readonly ILogger<PostDatabase> _logger;

        public PostDatabase(ILogger<PostDatabase> logger)
        {
            _logger = logger;
        }

        [Function("PostDatabase")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
