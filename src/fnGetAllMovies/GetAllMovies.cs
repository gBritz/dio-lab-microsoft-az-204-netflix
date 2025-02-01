using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace fnGetAllMovies
{
    public class GetAllMovies
    {
        private readonly ILogger<GetAllMovies> _logger;
        private readonly CosmosClient _cosmosClient;

        public GetAllMovies(
            ILogger<GetAllMovies> logger,
            CosmosClient cosmosClient)
        {
            _logger = logger;
            _cosmosClient = cosmosClient;
        }

        [Function("movies")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var container = _cosmosClient.GetContainer("DioFlixDb", "movies");
            var id = req.Query["id"];
            const string query = "SELECT * FROM c";
            var queryDefinition = new QueryDefinition(query);
            var queryIterator = container.GetItemQueryIterator<MovieResult>(queryDefinition);
            var movies = new List<MovieResult>();

            while (queryIterator.HasMoreResults)
            {
                foreach (var item in await queryIterator.ReadNextAsync())
                {
                    movies.Add(item);
                }
            }

            var responseMessage = req.CreateResponse(HttpStatusCode.OK);
            await responseMessage.WriteAsJsonAsync(movies);

            return responseMessage;
        }
    }
}
