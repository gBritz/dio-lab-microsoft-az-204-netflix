using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace fnPostDatabase
{
    public class PostDatabase
    {
        private readonly ILogger<PostDatabase> _logger;

        public PostDatabase(ILogger<PostDatabase> logger)
        {
            _logger = logger;
        }

        [Function("movie")]
        [CosmosDBOutput("%DatabaseName%", "%ContainerName%", Connection = "CosmosDbConnection", PartitionKey = "id", CreateIfNotExists = true)]
        public async Task<object> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            MovieRequest movie = null;

            var content = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                movie = JsonConvert.DeserializeObject<MovieRequest>(content);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult("Erro ao deserializar o objeto: " + ex.Message);
            }

            var jsonSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            return JsonConvert.SerializeObject(movie, jsonSettings);
        }
    }
}
