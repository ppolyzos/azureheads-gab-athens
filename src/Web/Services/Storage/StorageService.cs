using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using EventManagement.Api.Core.Infrastructure.Output;
using Microsoft.Extensions.Configuration;

namespace EventManagement.Web.Services.Storage
{
    public interface IStorageService
    {
        Task<T> FetchAsync<T>(string container, string fileConfig);
    }

    public class StorageService : IStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public StorageService(IConfiguration configuration)
        {
            _blobServiceClient = new BlobServiceClient(configuration.GetConnectionString("StorageAccount"));
        }

        public async Task<T> FetchAsync<T>(string container, string fileConfig)
        {
            if (string.IsNullOrEmpty(container) || string.IsNullOrEmpty(fileConfig)) return default;
            
            var containerClient = _blobServiceClient.GetBlobContainerClient(container);
            var blobClient = containerClient.GetBlobClient(fileConfig);
            bool exists = await blobClient.ExistsAsync();
            if (!exists) return default;

            await using var memoryStream = new MemoryStream();
            await blobClient.DownloadToAsync(memoryStream);
            var text = Encoding.UTF8.GetString(memoryStream.ToArray());
            return JsonSerializer.Deserialize<T>(text, SerializerSettings.Default);
        }
    }
}