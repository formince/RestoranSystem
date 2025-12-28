using Microsoft.AspNetCore.Mvc;
using Restoran.Core.Business;

namespace Restoran.Web.Controllers
{
    public class MenuController : Controller
    {

        private readonly ILogger<MenuController> _logger;
       
        public MenuController(ILogger<MenuController> logger)
        {
            _logger = logger;
        }

        [HttpGet]        
        public async  Task<IActionResult> Index()
        {
            _logger.LogInformation("➡️ DB bağlantısı deneniyor");
            var bll = new BLLProduct();
            var products = await bll.GetProductsAsync();
            ViewData["Title"] = "Menü";
            return View(products);
        }

        [HttpPost]
        public async  Task<IActionResult> Search(string keyword)
        {
            var bll = new BLLProduct();
            var results = await bll.Search(keyword); 
            ViewData["Title"] = $"Arama: {keyword}";
            return View("Index", results);
        }

        public async  Task<IActionResult> Details(int id)
        {
            var bll = new BLLProduct();
            var product = await bll.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            ViewData["Title"] = "Ürün Detayı";
            return View(product);
        }
    }
}
