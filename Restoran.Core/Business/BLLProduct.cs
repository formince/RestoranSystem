using Microsoft.EntityFrameworkCore;
using Restoran.Core.Data;
using Restoran.Core.DTOs.Product;
using Restoran.Core.Entity;
using Restoran.Core.Integrations.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restoran.Core.Business
{
    public class BLLProduct 
    {
        private readonly string _webRootPath;

        public BLLProduct(string webRootPath = "")
        {
            _webRootPath = webRootPath;
        }

        private RestaurantDbContext CreateContext()
        {
            return new RestoranDbContextFactory().CreateDbContext();
        }

        public async Task<List<ProductListDto>> GetProductsAsync()
        {
            using var context = CreateContext();

            var products = await context.Products
                                        .Include(p => p.Category)
                                        .ToListAsync();

            var productListDtos = new List<ProductListDto>();
            foreach (var product in products)
            {
                productListDtos.Add(new ProductListDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    CategoryName = product.Category?.Name!,
                    ImageUrl = FileService.GetFileUrl(product.ImageUrl),
                    StockQuantity = product.StockQuantity
                });
            }
            return productListDtos;
        }

        public async Task<ProductDetailDto?> GetProductByIdAsync(int id)
        {
            using var context = CreateContext();

            var product = await context.Products
                                        .Include(p => p.Category)
                                        .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return null;

            return new ProductDetailDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryName = product.Category?.Name!,
                ImageUrl = FileService.GetFileUrl(product.ImageUrl),
                StockQuantity = product.StockQuantity,
                DisplayOrder = product.DisplayOrder
            };
        }

        public async Task<(bool Success, string Message)> CreateProductAsync(ProductCreateDto dto, byte[]? imageData = null, string? imageFileName = null)
        {
            using var context = CreateContext();

            try
            {
                var product = new Product
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    CategoryId = dto.CategoryId,
                    Description = dto.Description,
                    StockQuantity = dto.StockQuantity,
                    DisplayOrder = 0
                };

                
                if (imageData != null && imageFileName != null)
                {
                    var fileResult = FileService.SaveFile(imageData, imageFileName, _webRootPath);
                    if (!fileResult.Success)
                    {
                        return (false, fileResult.Message);
                    }
                    product.ImageUrl = fileResult.FileName!;
                }

                await context.Products.AddAsync(product);
                var result = await context.SaveChangesAsync() > 0;

                return result ? (true, "Ürün başarıyla eklendi.") : (false, "Ürün eklenemedi.");
            }
            catch (Exception ex)
            {
                return (false, $"Hata: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UpdateProductAsync(int id, ProductUpdateDto dto, byte[]? imageData = null, string? imageFileName = null)
        {
            using var context = CreateContext();

            try
            {
                var product = await context.Products.FindAsync(id);
                if (product == null) 
                    return (false, "Ürün bulunamadı.");

                var oldImageUrl = product.ImageUrl;

                product.Name = dto.Name;
                product.Price = dto.Price;
                product.CategoryId = dto.CategoryId;
                product.Description = dto.Description;
                product.StockQuantity = dto.StockQuantity;

              
                if (imageData != null && imageFileName != null)
                {
                    var fileResult = FileService.SaveFile(imageData, imageFileName, _webRootPath);
                    if (!fileResult.Success)
                    {
                        return (false, fileResult.Message);
                    }
                    
                 
                    if (!string.IsNullOrEmpty(oldImageUrl))
                    {
                        FileService.DeleteFile(oldImageUrl, _webRootPath);
                    }
                    
                    product.ImageUrl = fileResult.FileName!;
                }

                var result = await context.SaveChangesAsync() > 0;
                return result ? (true, "Ürün başarıyla güncellendi.") : (false, "Ürün güncellenemedi.");
            }
            catch (Exception ex)
            {
                return (false, $"Hata: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeleteProductAsync(int id)
        {
            using var context = CreateContext();

            try
            {
                var product = await context.Products.FindAsync(id);
                if (product == null) 
                    return (false, "Ürün bulunamadı.");

                product.IsActive = false;

                var result = await context.SaveChangesAsync() > 0;
                
             
                if (result && !string.IsNullOrEmpty(product.ImageUrl))
                {
                    FileService.DeleteFile(product.ImageUrl, _webRootPath);
                }

                return result ? (true, "Ürün başarıyla silindi.") : (false, "Ürün silinemedi.");
            }
            catch (Exception ex)
            {
                return (false, $"Hata: {ex.Message}");
            }
        }
    }
}
