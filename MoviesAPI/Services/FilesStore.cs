using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MoviesAPI.Services
{
    public class FilesStore : IFilesStore
    {
        private readonly string _connectionString;

        public FilesStore(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AzureStorage");
        }

        public async Task DeleteFile(string path, string container)
        {
            if(path != null)
            {
                var account = CloudStorageAccount.Parse(_connectionString);
                var client = account.CreateCloudBlobClient();
                var containerReference = client.GetContainerReference(container);

                var nameBlob = Path.GetFileName(path);
                var blob = containerReference.GetBlobReference(nameBlob);
                await blob.DeleteIfExistsAsync();
            }
        }

        public async Task<string> EditFile(byte[] content, string extension, string container, string path, string contentType)
        {
            await DeleteFile(path, container);
            return await SaveFile(content, extension, container, contentType);
        }

        public async Task<string> SaveFile(byte[] content, string extension, string container, string contentType)
        {
            var account = CloudStorageAccount.Parse(_connectionString);
            var client = account.CreateCloudBlobClient();
            var containerReference = client.GetContainerReference(container);

            await containerReference.CreateIfNotExistsAsync();
            await containerReference.SetPermissionsAsync(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });
            
            var nameFile = $"{Guid.NewGuid()}{extension}";
            var blob = containerReference.GetBlockBlobReference(nameFile);
            await blob.UploadFromByteArrayAsync(content, 0, content.Length);
            blob.Properties.ContentType = contentType;
            await blob.SetPropertiesAsync();
            return blob.Uri.ToString();
        }
    }
}
