using Azure.Storage;
using Microsoft.WindowsAzure.Storage;

namespace e_citaonica_api.Services;
public class FileService : IFileService
{
    private readonly CloudStorageAccount cloudStorageAccount;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IConfiguration _configuration;
    public FileService(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
    {
        _webHostEnvironment = webHostEnvironment;
        _configuration = configuration;
        cloudStorageAccount = CloudStorageAccount.Parse(_configuration["ConnectionStrings:BlobStorage"]);
    }

    public string DeleteFile(string path)
    {
        if (String.IsNullOrEmpty(path))
            return "Fajl ne postoji";

        path = Path.GetFileName(path);

        var blobClient = cloudStorageAccount.CreateCloudBlobClient();
        var blobContainer = blobClient.GetContainerReference("container1");
        var res = blobContainer.GetBlockBlobReference(path).DeleteIfExistsAsync().Result;

        if (res == true)
            return "Fajl obrisan.";
        else
            return "Fajl ne postoji.";
    }

    public async Task<string> SaveFile(IFormFile file, string relativePath = "common")
    {
        string path = Path.Combine(_webHostEnvironment.ContentRootPath, "Files", relativePath);

        string newFileName = new PasswordGenerator.Password(
                includeLowercase: true, 
                includeUppercase: true, 
                passwordLength: 100, 
                includeSpecial: false,
                includeNumeric: true).Next();
        newFileName += DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(file.FileName);

        var blobClient = cloudStorageAccount.CreateCloudBlobClient();
        var blobContainer = blobClient.GetContainerReference("container1");

        if (await blobContainer.CreateIfNotExistsAsync())
        {
            await blobContainer.SetPermissionsAsync(new Microsoft.WindowsAzure.Storage.Blob.BlobContainerPermissions
            {
                PublicAccess = Microsoft.WindowsAzure.Storage.Blob.BlobContainerPublicAccessType.Container
            });
        }

        var blobBlock = blobContainer.GetBlockBlobReference(newFileName);
        blobBlock.Properties.ContentType = file.ContentType;

        await blobBlock.UploadFromStreamAsync(file.OpenReadStream());

        return $"{blobClient.BaseUri.AbsoluteUri}container1/{newFileName}";
    }

    async public Task<string> SaveImage(IFormFile file)
    {
        if (IsImage(file))
        {
            return await SaveFile(file, "images");
        }

        throw new Exception("File is not an image");
    }

    bool IsImage(IFormFile image)
    {
        if (image.ContentType.ToLower() != "image/jpg" &&
            image.ContentType.ToLower() != "image/jpeg" &&
            image.ContentType.ToLower() != "image/pjpeg" &&
            image.ContentType.ToLower() != "image/x-png" &&
            image.ContentType.ToLower() != "image/png" &&
            image.ContentType.ToLower() != "image/webp")
        {
            return false;
        }

        if (Path.GetExtension(image.FileName).ToLower() != ".jpg"
            && Path.GetExtension(image.FileName).ToLower() != ".png"
            && Path.GetExtension(image.FileName).ToLower() != ".jpeg"
            && Path.GetExtension(image.FileName).ToLower() != ".webp")
        {
            return false;
        }

        return true;
    }

    public void DeleteFileNoAwait(string path)
    {
        if (String.IsNullOrEmpty(path))
            return;

        path = Path.GetFileName(path);

        var blobClient = cloudStorageAccount.CreateCloudBlobClient();
        var blobContainer = blobClient.GetContainerReference("container1");
        blobContainer.GetBlockBlobReference(path).DeleteIfExistsAsync();
    }
}