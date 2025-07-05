using MyWebApi.Models;
using System.Collections.Generic;

namespace MyWebApi.DTOs.Conversions
{
    public static class ProductConversion
    {
        public static Product ToEntity(ProductDTO product) => new Product
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity,
            Size = product.Size,
            Color = product.Color,
            Material = product.Material,
            CreatedAt = DateTime.Now, // Assuming CreatedAt is set to now on creation
            UpdatedAt = DateTime.Now,  // Assuming UpdatedAt is set to now on creation
            CategoryId = product.CategoryId
        };
        public static (ProductDTO?, IEnumerable<ProductDTO>?) FromEntity(Product product, IEnumerable<Product>? products)
        {
            //return single 
            if (product is not null || products is null)
            {
                var singleProduct = new ProductDTO(
                   product!.Id,
                   product.Name,
                   product.Description,
                   product.Price,
                   product.Quantity,
                   product.Size,
                   product.Color,
                   product.Material,
                   product.CategoryId
                );
                return (singleProduct, null);
            }
            //return lists
            if (products is not null || product is null)
            {
                var productList = products!.Select(p =>
                
                    new ProductDTO(
                        p.Id,
                        p.Name,
                        p.Description,
                        p.Price,
                        p.Quantity,
                        p.Size,
                        p.Color,
                        p.Material,
                        p.CategoryId
                )).ToList();
                return (null, productList);
            }
            return (null, null);
        }
    }
}
