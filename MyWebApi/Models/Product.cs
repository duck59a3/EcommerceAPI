﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApi.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(255)")]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Description { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public int Quantity { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string Size { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string Color { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Material { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;


        // Navigation properties
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<ProductVideo> Videos { get; set; } = new List<ProductVideo>();
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }
    }
}
