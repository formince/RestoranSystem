using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restoran.Core.Business;
using Restoran.Core.DTOs.Category;
using Restoran.Core.DTOs.Product;
using Restoran.Core.DTOs.Table;
using Restoran.Core.DTOs.User;
using System.Drawing;

namespace Restoran.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Yönetim Paneli";
            return View();
        }
        // ürün işlemleri
        public async Task<IActionResult> Products()
        {
            var bll = new BLLProduct();
            var list =  await bll.GetProductsAsync();
            ViewData["Title"] = "Ürün Yönetimi";
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> AddProduct()
        {
            ViewData["Title"] = "Ürün Ekle";
            
            // Kategorileri çekip ViewBag'e koy
            var categoryBll = new BLLCategory();
            var categories = await categoryBll.GetCategoriesAsync();
            ViewBag.Categories = categories;
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductCreateDto dto, IFormFile? imageData)
        {
            if (!ModelState.IsValid)
            {
                // Hata durumunda kategorileri tekrar yükle
                var categoryBll = new BLLCategory();
                var categories = await categoryBll.GetCategoriesAsync();
                ViewBag.Categories = categories;
                
                ViewData["Title"] = "Yeni Ürün Ekle";
                return View(dto);
            }

            var bll = new BLLProduct();

            // Resim dosyasını byte[]'a dönüştürelim
            byte[]? imageBytes = null;
            string? imageFileName = null;

            if (imageData != null)
            {
                using var memoryStream = new MemoryStream();
                await imageData.CopyToAsync(memoryStream);
                imageBytes = memoryStream.ToArray();
                imageFileName = imageData.FileName;
            }

            var result = await bll.CreateProductAsync(dto, imageBytes, imageFileName);

            if (!result.Success)
            {
                // Hata durumunda kategorileri tekrar yükle
                var categoryBll = new BLLCategory();
                var categories = await categoryBll.GetCategoriesAsync();
                ViewBag.Categories = categories;
                
                ModelState.AddModelError("", result.Message);
                ViewData["Title"] = "Yeni Ürün Ekle";
                return View(dto);
            }

            return RedirectToAction("Products");
        }


        [HttpGet]
        public async Task<IActionResult> UpdateProduct(int id)
        {
            var bll = new BLLProduct();

            // Ürün detaylarını çek
            var productUpdateDto = await bll.GetProductByIdAsync(id);

            if (productUpdateDto == null)
                return NotFound();
           

            // Kategorileri çekip ViewBag'e koy
            var categoryBll = new BLLCategory();
            var categories = await categoryBll.GetCategoriesAsync();
            ViewBag.Categories = categories;

            ViewData["Title"] = "Ürün Güncelle";
            return View(productUpdateDto);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(int id, ProductUpdateDto dto, IFormFile? imageData)
        {
            if(!ModelState.IsValid)
            {
                
                var categoryBll = new BLLCategory();
                var categories = await categoryBll.GetCategoriesAsync();
                ViewBag.Categories = categories;
                
                ViewData["Title"] = "Ürün Güncelle";
                return View("UpdateProduct", dto);
            }

            var bll = new BLLProduct();

            // Resim dosyasını byte[]'a dönüştürelim
            byte[]? imageBytes = null;
            string? imageFileName = null;

            if (imageData != null)
            {
                using var memoryStream = new MemoryStream();
                await imageData.CopyToAsync(memoryStream);
                imageBytes = memoryStream.ToArray();
                imageFileName = imageData.FileName;
            }

            var result = await bll.UpdateProductAsync(id, dto, imageBytes, imageFileName);

            if (result.Success)
            {
                return RedirectToAction("Products"); // Başarılıysa ürün listesine yönlendir
            }

            // Hata durumunda kategorileri tekrar yükle
            var categoryBllError = new BLLCategory();
            var categoriesError = await categoryBllError.GetCategoriesAsync();
            ViewBag.Categories = categoriesError;
            
            ModelState.AddModelError("", result.Message);
            ViewData["Title"] = "Ürün Güncelle";
            return View("UpdateProduct", dto); // Hata varsa formu tekrar göster
        }

        public async  Task<IActionResult> DeleteProduct(int id)
        {
            if(!ModelState.IsValid)
            {
                ViewData["Title"] = "Ürün Sil";
                return View();
            }

            var bll = new BLLProduct();
            await bll.DeleteProductAsync(id);
            return RedirectToAction("Products");
        }


        // rezervasyon işlemleri
        [HttpGet]
        public async Task<IActionResult> Reservations()
        {
            var bll = new BLLReservation();
            var list = await bll.GetReservationsAsync(); // tüm rezervasyonları getir
            ViewData["Title"] = "Rezervasyonlar";
            return View(list);
        }
        [HttpGet]
        public async Task<IActionResult> ReservationDetails(int id)
        {
            if(!ModelState.IsValid)
            {
                ViewData["Title"] = "Rezervasyon Detayı";
                return View();
            }

            var bll = new BLLReservation();
            var res = await bll.GetReservationByIdAsync(id);

            if (res == null)
                return NotFound();

            ViewData["Title"] = "Rezervasyon Detayı";
            return View(res);
        }
        [HttpPost]
        public async Task<IActionResult> CancelReservation(int id)
        {
            if(!ModelState.IsValid)
            {
                ViewData["Title"] = "Rezervasyon İptal";
                return View();
            }

            var bll = new BLLReservation();
             await bll.DeleteReservationAsync(id);
            return RedirectToAction("Reservations");
        }

        // sipariş işlemleri

        public async Task<IActionResult> Orders()
        {
            var bll = new BLLOrder();
            var orders = await bll.GetOrdersAsync(); 
            ViewData["Title"] = "Siparişler";
            return View(orders);
        }

        public async  Task<IActionResult> OrderDetails(int id)
        {       if(!ModelState.IsValid)
            {
                ViewData["Title"] = "Sipariş Detayı";
                return View();
            }

            var bll = new BLLOrder();
            var order =  await bll.GetOrderByIdAsync(id);

            if (order == null)
                return NotFound();

            ViewData["Title"] = "Sipariş Detayı";
            return View(order);
        }

        // masa işlemleri
        public async Task<IActionResult> Tables()
        {
            var bll = new BLLTable();
            var list = await bll.GetTablesAsync();
            ViewData["Title"] = "Masa Yönetimi";
            return View(list);
        }

        [HttpGet]
        public IActionResult AddTable()
        {
            ViewData["Title"] = "Masa Ekle";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddTable(TableCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Masa Ekle";
                return View(dto);
            }

            var bll = new BLLTable();
            var result = await bll.CreateTableAsync(dto);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                ViewData["Title"] = "Masa Ekle";
                return View(dto);
            }

            return RedirectToAction("Tables");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateTable(int id)
        {
            var bll = new BLLTable();
            var tableUpdateDto = await bll.GetTableByIdAsync(id);

            if (tableUpdateDto == null)
                return NotFound();            

            ViewData["Title"] = "Masa Güncelle";
            return View(tableUpdateDto);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTable(int id, TableUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Masa Güncelle";
                return View("UpdateTable", dto);
            }

            var bll = new BLLTable();
            var result = await bll.UpdateTableAsync(id, dto);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                ViewData["Title"] = "Masa Güncelle";
                return View("UpdateTable", dto);
            }

            return RedirectToAction("Tables");
        }

        public async Task<IActionResult> DeleteTable(int id)
        {
            var bll = new BLLTable();
            await bll.DeleteTableAsync(id);
            return RedirectToAction("Tables");
        }

        // kullanıcı işlemleri
        public async Task<IActionResult> Users()
        {
            var bll = new BLLUser();
            var list = await bll.GetUsersAsync();
            ViewData["Title"] = "Kullanıcı Yönetimi";
            return View(list);
        }

        [HttpGet]
        public IActionResult AddUser()
        {
            ViewData["Title"] = "Kullanıcı Ekle";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(UserRegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Kullanıcı Ekle";
                return View(dto);
            }

            var bll = new BLLUser();
            var result = await bll.CreateUserAsync(dto);

            if (!result)
            {
                ModelState.AddModelError("", "Kullanıcı oluşturulamadı. Username veya email zaten kullanılıyor olabilir.");
                ViewData["Title"] = "Kullanıcı Ekle";
                return View(dto);
            }

            return RedirectToAction("Users");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateUser(int id)
        {
            var bll = new BLLUser();
            var userDto = await bll.GetUserByIdAsync(id);

            if (userDto == null)
                return NotFound();
        

            ViewData["Title"] = "Kullanıcı Güncelle";
            return View(userDto);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(int id, UserUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Kullanıcı Güncelle";
                return View("EditUser", dto);
            }

            var bll = new BLLUser();
            var result = await bll.UpdateUserAsync(id, dto);

            if (!result)
            {
                ModelState.AddModelError("", "Kullanıcı güncellenemedi.");
                ViewData["Title"] = "Kullanıcı Güncelle";
                return View("EditUser", dto);
            }

            return RedirectToAction("Users");
        }

        public async Task<IActionResult> DeleteUser(int id)
        {
            var bll = new BLLUser();
            await bll.DeleteUserAsync(id);
            return RedirectToAction("Users");
        }

        [HttpGet]
        public async Task<IActionResult> UserDetails(int id)
        {
            var bll = new BLLUser();
            var user = await bll.GetUserByIdAsync(id);

            if (user == null)
                return NotFound();

            ViewData["Title"] = "Kullanıcı Detayı";
            return View(user);
        }

        // kategori işlemleri
        public async Task<IActionResult> Categories()
        {
            var bll = new BLLCategory();
            var list = await bll.GetCategoriesAsync();
            ViewData["Title"] = "Kategori Yönetimi";
            return View(list);
        }

        [HttpGet]
        public IActionResult AddCategory()
        {
            ViewData["Title"] = "Kategori Ekle";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryCreateDto dto, IFormFile? imageData)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Kategori Ekle";
                return View(dto);
            }

            var bll = new BLLCategory();

            // Resim dosyasını byte[]'a dönüştürelim
            byte[]? imageBytes = null;
            string? imageFileName = null;

            if (imageData != null)
            {
                using var memoryStream = new MemoryStream();
                await imageData.CopyToAsync(memoryStream);
                imageBytes = memoryStream.ToArray();
                imageFileName = imageData.FileName;
            }

            var result = await bll.CreateCategoryAsync(dto, imageBytes, imageFileName);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                ViewData["Title"] = "Kategori Ekle";
                return View(dto);
            }

            return RedirectToAction("Categories");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCategory(int id)
        {
            var bll = new BLLCategory();
            var category = await bll.GetCategoryByIdAsync(id);

            if (category == null)
                return NotFound();

            ViewData["Title"] = "Kategori Güncelle";
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory(int id, CategoryUpdateDto dto, IFormFile? imageData)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Kategori Güncelle";
                return View("EditCategory", dto);
            }

            var bll = new BLLCategory();

            // Resim dosyasını byte[]'a dönüştürelim
            byte[]? imageBytes = null;
            string? imageFileName = null;

            if (imageData != null)
            {
                using var memoryStream = new MemoryStream();
                await imageData.CopyToAsync(memoryStream);
                imageBytes = memoryStream.ToArray();
                imageFileName = imageData.FileName;
            }

            var result = await bll.UpdateCategoryAsync(id, dto, imageBytes, imageFileName);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                ViewData["Title"] = "Kategori Güncelle";
                return View("EditCategory", dto);
            }

            return RedirectToAction("Categories");
        }

        public async Task<IActionResult> DeleteCategory(int id)
        {
            var bll = new BLLCategory();
            var result = await bll.DeleteCategoryAsync(id);
            
            if (!result.Success)
            {
                // Hata mesajını TempData ile gönder
                TempData["ErrorMessage"] = result.Message;
            }
            
            return RedirectToAction("Categories");
        }
    }
}
