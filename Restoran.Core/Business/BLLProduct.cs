using Microsoft.EntityFrameworkCore;
using Restoran.Core.Data;
using Restoran.Core.DTOs.Product;
using Restoran.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restoran.Core.Business
{
    public class BLLProduct // Sınıf adı BLLProduct olarak değiştirildi
    {
        public BLLProduct()
        {
            // DI ile ilgili hiçbir şey burada olmayacak.
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
                    CategoryName = product.Category?.Name!
                    
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
                CategoryName = product.Category?.Name!
               
            };
        }

        public async Task<bool> CreateProductAsync(ProductCreateDto dto)
        {
            using var context = CreateContext();

            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                Description = dto.Description
            };
            await context.Products.AddAsync(product);

            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateProductAsync(int id, ProductUpdateDto dto)
        {
            using var context = CreateContext();

            var product = await context.Products.FindAsync(id);
            if (product == null) return false;

            product.Name = dto.Name;
            product.Price = dto.Price;
            product.CategoryId = dto.CategoryId;
            product.Description = dto.Description;

            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            using var context = CreateContext();

            var product = await context.Products.FindAsync(id);
            if (product == null) return false;

            product.IsActive = false;

            return await context.SaveChangesAsync() > 0;
        }
    }
}
