using Restoran.Core.DTOs.Category;

namespace Restoran.Core.Business.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryListDto>> GetAllAsync();
        Task<CategoryListDto?> GetByIdAsync(int id);
        Task<bool> CreateAsync(CategoryCreateDto dto);
        Task<bool> UpdateAsync(int id, CategoryUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        
        // Kategori'ye özel method
        Task<bool> IsCategoryNameExistsAsync(string name, int? excludeId = null);
    }
}