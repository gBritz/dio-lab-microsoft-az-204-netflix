using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("Processando a imagem no azure container storage.");

            if (!req.Headers.TryGetValue("file-type", out var fileTypeHeader))
            {
                return new BadRequestObjectResult("O cabeçalho 'file-type' é obrigatório.");
            }

            var fileType = fileTypeHeader.ToString();
            var form = await req.ReadFormAsync();
            var file = form.Files["file"];

            if (file is null || file.Length == 0)
            {
                return new BadRequestObjectResult("O arquivo não foi enviado.");
            }

            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            var containerName = fileType;
            BlobClient blobClient = new(connectionString, containerName, file.FileName);
            BlobContainerClient blobContainerCLient = new(connectionString, containerName);

            await blobContainerCLient.CreateIfNotExistsAsync();
            await blobContainerCLient.SetAccessPolicyAsync(PublicAccessType.BlobContainer);


            var blobName = file.FileName;
            var blob = blobContainerCLient.GetBlobClient(blobName);

            using (var stream = file.OpenReadStream())
            {
                await blob.UploadAsync(stream, true);
            }

            _logger.LogInformation($"Arquivo {file.FileName} armazenado com sucesso.");

            return new OkObjectResult(new
            {
                Message = "Arquivo armazenado com sucesso.",
                BlobUri = blob.Uri,
            });
        }
    }
}
