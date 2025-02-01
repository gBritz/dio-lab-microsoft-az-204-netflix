using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace fnGetMovieDetail
{
    public class GetMovieDetail
    {
        private readonly ILogger<GetMovieDetail> _logger;
        private readonly CosmosClient _cosmosClient;

        public GetMovieDetail(
            ILogger<GetMovieDetail> logger,
            CosmosClient cosmosClient)
        {
            _logger = logger;
            _cosmosClient = cosmosClient;
        }

        [Function("detail")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var container = _cosmosClient.GetContainer("DioFlixDb", "movies");
            var id = req.Query["id"];
            const string query = "SELECT * FROM c WHERE c.id = @id";
            var queryDefinition = new QueryDefinition(query)
                .WithParameter("@id", id);
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
            await responseMessage.WriteAsJsonAsync(movies.FirstOrDefault());

            return responseMessage;
        }
    }
}
