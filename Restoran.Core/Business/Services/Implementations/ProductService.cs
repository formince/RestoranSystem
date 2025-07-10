using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restoran.Core.Business.Services.Interfaces;
using Restoran.Core.DTOs.Product;
using Restoran.Core.Statics;
using Restoran.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restoran.Core.Entity;

namespace Restoran.Core.Business.Services.Implementations
{
    public class ProductService : IProductService // Arayüzü uyguluyor
    {
        private readonly IMapper _mapper;

        // Constructor: Sadece IMapper'ı alıyor, bağlantı dizesini AppConfiguration'dan çekecek
        public ProductService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<List<ProductListDto>> GetProductsAsync()
        {
            // Using pattern ile doğrudan bağlantı - proje gereksinimine uygun
            using var context = AppConfiguration.CreateDbContext();
            
            var products = await context.Products
                                      .Include(p => p.Category)
                                      .ToListAsync();

            return _mapper.Map<List<ProductListDto>>(products);
        }

        // Diğer CRUD işlemleri
        public async Task<ProductDetailDto?> GetProductByIdAsync(int id)
        {
            using var context = AppConfiguration.CreateDbContext();
            
            var product = await context.Products
                                      .Include(p => p.Category)
                                      .FirstOrDefaultAsync(p => p.Id == id);

            return product != null ? _mapper.Map<ProductDetailDto>(product) : null;
        }

        public async Task<bool> CreateProductAsync(ProductCreateDto dto)
        {
            using var context = AppConfiguration.CreateDbContext();
            
            var product = _mapper.Map<Product>(dto);
            await context.Products.AddAsync(product);
            
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateProductAsync(int id, ProductUpdateDto dto)
        {
            using var context = AppConfiguration.CreateDbContext();
            
            var product = await context.Products.FindAsync(id);
            if (product == null) return false;

            _mapper.Map(dto, product);
            
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            using var context = AppConfiguration.CreateDbContext();
            
            var product = await context.Products.FindAsync(id);
            if (product == null) return false;

            // Soft delete
            product.IsActive = false;
            
            return await context.SaveChangesAsync() > 0;
        }
    }
}
