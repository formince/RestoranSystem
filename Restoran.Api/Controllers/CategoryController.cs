using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restoran.Core.Business;
using Restoran.Core.DTOs.Category;

namespace Restoran.Api.Controllers
{
    public class CategoryController : BaseController
    {

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategories()
        {
            var categoryBLL = new BLLCategory(WebRootPath);
            var categories = await categoryBLL.GetCategoriesAsync();
            return HandleResult(categories, "Kategoriler başarıyla getirildi");
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategory(int id)
        {
            var categoryBLL = new BLLCategory(WebRootPath);
            var category = await categoryBLL.GetCategoryByIdAsync(id);
            return HandleResult(category, "Kategori başarıyla getirildi");
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateCategory([FromForm] CategoryCreateDto dto, IFormFile? image)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            byte[]? imageData = null;
            string? imageFileName = null;

            // Resim var mı kontrol et
            if (image != null && image.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await image.CopyToAsync(memoryStream);
                imageData = memoryStream.ToArray();
                imageFileName = image.FileName;
            }

            var categoryBLL = new BLLCategory(WebRootPath);
            var result = await categoryBLL.CreateCategoryAsync(dto, imageData, imageFileName);
            return HandleResult(result);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateCategory(int id, [FromForm] CategoryUpdateDto dto, IFormFile? image)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            byte[]? imageData = null;
            string? imageFileName = null;

            // Resim var mı kontrol et
            if (image != null && image.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await image.CopyToAsync(memoryStream);
                imageData = memoryStream.ToArray();
                imageFileName = image.FileName;
            }

            var categoryBLL = new BLLCategory(WebRootPath);
            var result = await categoryBLL.UpdateCategoryAsync(id, dto, imageData, imageFileName);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var categoryBLL = new BLLCategory(WebRootPath);
            var result = await categoryBLL.DeleteCategoryAsync(id);
            return HandleResult(result);
        }
    }
} 