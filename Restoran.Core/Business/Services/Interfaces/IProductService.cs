using Restoran.Core.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restoran.Core.Business.Services.Interfaces
{
    public interface IProductService
    {
        // Ürün listesini ProductListDto olarak döndüren asenkron metod
        Task<List<ProductListDto>> GetProductsAsync();

        Task<ProductDetailDto?> GetProductByIdAsync(int id);

        Task<bool> CreateProductAsync(ProductCreateDto dto);

        Task<bool> UpdateProductAsync(int id, ProductUpdateDto dto);

        Task<bool> DeleteProductAsync(int id);

        // İleride eklenebilecek metod örnekleri:
        // Task<ProductDetailDto> GetProductByIdAsync(int id);
        // Task AddProductAsync(ProductCreateDto productDto);
        // Task UpdateProductAsync(ProductUpdateDto productDto);
        // Task DeleteProductAsync(int id);
    }
}
