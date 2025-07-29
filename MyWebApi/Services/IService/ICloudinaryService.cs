using CloudinaryDotNet.Actions;

namespace MyWebApi.Services.IService
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadImageAsync(IFormFile file, string folder = "ecommerce");
        Task<VideoUploadResult> UploadVideoAsync(IFormFile file, string folder = "ecommerce");
        Task<DeletionResult> DeleteImageAsync(string publicId);
        Task<List<ImageUploadResult>> UploadMultipleImagesAsync(IFormFileCollection files, string folder = "ecommerce");
        string GetOptimizedImageUrl(string publicId, int width = 0, int height = 0);
    }
}
