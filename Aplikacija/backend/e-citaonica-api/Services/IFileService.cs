
namespace e_citaonica_api.Services
{
    public interface IFileService
    {
        Task<string> SaveFile(IFormFile file, string relativePath = "common");
        Task<string> SaveImage(IFormFile file);
        string DeleteFile(string path);
        void DeleteFileNoAwait(string path);
    }
}