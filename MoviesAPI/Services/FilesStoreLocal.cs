using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Services
{
    public class FilesStoreLocal : IFilesStore
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FilesStoreLocal(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task DeleteFile(string path, string container)
        {
            if(path != null)
            {
                var nameFile = Path.GetFileName(path);
                string folderFile = Path.Combine(_env.WebRootPath, container, nameFile);

                if (File.Exists(folderFile))
                    File.Delete(folderFile);
            }
            return Task.FromResult(0);
        }

        public async Task<string> EditFile(byte[] content, string extension, string container, string path, string contentType)
        {
            await DeleteFile(path, container);
            return await SaveFile(content, extension, container, contentType);
        }

        public async Task<string> SaveFile(byte[] content, string extension, string container, string contentType)
        {
            var nameFile = $"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(_env.WebRootPath, container);

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string path = Path.Combine(folder, nameFile);
            await File.WriteAllBytesAsync(path, content);

            var urlDomine = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";

            var url = Path.Combine(urlDomine, container, nameFile).Replace("\\", "/");

            return url;
        }
    }
}
