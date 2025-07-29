using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using MyWebApi.Helpers;
using MyWebApi.Services.IService;

namespace MyWebApi.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinaryOptions> options)
        {
            var account = new Account(
                options.Value.CloudName,
                options.Value.ApiKey,
                options.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }
        public async Task<DeletionResult> DeleteImageAsync(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
                throw new ArgumentException("Public ID không thể rỗng hoặc để trống");

            var delParams = new DeletionParams(publicId);
            return await _cloudinary.DestroyAsync(delParams);
        }

        public string GetOptimizedImageUrl(string publicId, int width = 0, int height = 0)
        {
            var transformation = new Transformation()
                .Quality("auto")
                .FetchFormat("auto");

            if (width > 0)
                transformation = transformation.Width(width);

            if (height > 0)
                transformation = transformation.Height(height);

            return _cloudinary.Api.UrlImgUp.Transform(transformation).BuildUrl(publicId);
        }

        public async Task<ImageUploadResult> UploadImageAsync(IFormFile file, string folder = "ecommerce")
        {
            if (file == null || file.Length == 0) {
                throw new ArgumentException("File rỗng hoặc không tồn tại");

            }
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
            {
                throw new ArgumentException("Định dạng file không hợp lệ");
            }
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder,
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false,
                Transformation = new Transformation()
                    .Quality("auto")
                    .FetchFormat("auto")
            };
            return await _cloudinary.UploadAsync(uploadParams);
        }

        public async Task<List<ImageUploadResult>> UploadMultipleImagesAsync(IFormFileCollection files, string folder = "ecommerce")
        {
            var results = new List<ImageUploadResult>();

            foreach (var file in files)
            {
                try
                {
                    var result = await UploadImageAsync(file, folder);
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    // Log lỗi, tiếp tục các file khac
                    Console.WriteLine($"Error uploading file {file.FileName}: {ex.Message}");
                }
            }

            return results;
        }

        public async Task<VideoUploadResult> UploadVideoAsync(IFormFile file, string folder = "ecommerce")
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File rỗng hoặc không tồn tại");
            }
            var allowedTypes = new[] { "video/mp4", "video/avi", "video/mov", "video/wmv" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
            {
                throw new ArgumentException("Định dạng file không hợp lệ");
            }
            using var stream = file.OpenReadStream();
            var uploadParams = new VideoUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder,
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false
            };

            return await _cloudinary.UploadAsync(uploadParams);
        }
    }
}
