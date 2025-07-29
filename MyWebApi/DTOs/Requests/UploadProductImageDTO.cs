using Microsoft.Identity.Client;

namespace MyWebApi.DTOs.Requests
{
    public record UploadProductImageDTO(
        int productId,
        IFormFile image);

}
