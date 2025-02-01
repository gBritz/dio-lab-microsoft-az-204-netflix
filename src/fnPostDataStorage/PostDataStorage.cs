using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace fnPostDataStorage
{
    public class PostDataStorage
    {
        private readonly ILogger<PostDataStorage> _logger;

        public PostDataStorage(ILogger<PostDataStorage> logger)
        {
            _logger = logger;
        }

        [Function("PostDataStorage")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
