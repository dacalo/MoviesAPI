using System.Threading.Tasks;

namespace MoviesAPI.Services
{
    public interface IFilesStore
    {
        Task<string> EditFile(byte[] content, string extension, string container, string path, string contentType);
        Task DeleteFile(string path, string container);
        Task<string> SaveFile(byte[] content, string extension, string container, string contentType);
    }
}
