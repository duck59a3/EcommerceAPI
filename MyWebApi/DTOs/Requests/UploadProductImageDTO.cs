using Microsoft.Identity.Client;

namespace MyWebApi.DTOs.Requests
{
    public record UploadProductImageDTO(
        int productId,
        List<IFormFile> image);

}
