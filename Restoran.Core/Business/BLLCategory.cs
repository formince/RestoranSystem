using Microsoft.EntityFrameworkCore;
using Restoran.Core.Data;
using Restoran.Core.DTOs.Category;
using Restoran.Core.Entity;
using Restoran.Core.Integrations.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restoran.Core.Business
{
    public class BLLCategory 
    {
        private readonly string _webRootPath;

        public BLLCategory(string webRootPath = "")
        {
            _webRootPath = webRootPath;
        }

        private RestaurantDbContext CreateContext()
        {
            return new RestoranDbContextFactory().CreateDbContext();
        }

        public async Task<List<CategoryListDto>> GetCategoriesAsync()
        {
            using var context = CreateContext();

            var categories = await context.Categories.ToListAsync();

            var categoryListDtos = new List<CategoryListDto>();
            foreach (var category in categories)
            {
                categoryListDtos.Add(new CategoryListDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    DisplayOrder = category.DisplayOrder,
                    ImageUrl = FileService.GetFileUrl(category.ImageUrl)
                });
            }
            return categoryListDtos;
        }

        public async Task<CategoryUpdateDto?> GetCategoryByIdAsync(int id)
        {
            using var context = CreateContext();

            var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return null;

            return new CategoryUpdateDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                DisplayOrder = category.DisplayOrder,
                ImageUrl = FileService.GetFileUrl(category.ImageUrl)
            };
        }

        public async Task<(bool Success, string Message)> CreateCategoryAsync(CategoryCreateDto dto, byte[]? imageData = null, string? imageFileName = null)
        {
            using var context = CreateContext();

            try
            {
                var category = new Category
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    DisplayOrder = dto.DisplayOrder
                };

               
                if (imageData != null && imageFileName != null)
                {
                    var fileResult = FileService.SaveFile(imageData, imageFileName, _webRootPath);
                    if (!fileResult.Success)
                    {
                        return (false, fileResult.Message);
                    }
                    category.ImageUrl = fileResult.FileName!;
                }

                await context.Categories.AddAsync(category);
                var result = await context.SaveChangesAsync() > 0;

                return result ? (true, "Kategori başarıyla eklendi.") : (false, "Kategori eklenemedi.");
            }
            catch (Exception ex)
            {
                return (false, $"Hata: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UpdateCategoryAsync(int id, CategoryUpdateDto dto, byte[]? imageData = null, string? imageFileName = null)
        {
            using var context = CreateContext();

            try
            {
                var category = await context.Categories.FindAsync(id);
                if (category == null) 
                    return (false, "Kategori bulunamadı.");

                var oldImageUrl = category.ImageUrl;

                category.Name = dto.Name;
                category.Description = dto.Description;
                category.DisplayOrder = dto.DisplayOrder;

                
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
                    
                    category.ImageUrl = fileResult.FileName!;
                }

                var result = await context.SaveChangesAsync() > 0;
                return result ? (true, "Kategori başarıyla güncellendi.") : (false, "Kategori güncellenemedi.");
            }
            catch (Exception ex)
            {
                return (false, $"Hata: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeleteCategoryAsync(int id)
        {
            using var context = CreateContext();

            try
            {
                var category = await context.Categories.FindAsync(id);
                if (category == null) 
                    return (false, "Kategori bulunamadı.");

                category.IsActive = false;

                var result = await context.SaveChangesAsync() > 0;
                
               
                if (result && !string.IsNullOrEmpty(category.ImageUrl))
                {
                    FileService.DeleteFile(category.ImageUrl, _webRootPath);
                }

                return result ? (true, "Kategori başarıyla silindi.") : (false, "Kategori silinemedi.");
            }
            catch (Exception ex)
            {
                return (false, $"Hata: {ex.Message}");
            }
        }
    }
} 