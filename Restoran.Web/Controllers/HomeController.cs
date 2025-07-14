using Microsoft.AspNetCore.Mvc;
using Restoran.Core.Data;
using Restoran.Core.DTOs.Reservation;
using Restoran.Web.Models;
using System.Diagnostics;
using Restoran.Core.Business;

namespace Restoran.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> CreateReservation(ReservationCreateDto reservationCreateDto)
        {
           var bllreservation = new BLLReservation();
              var result = await bllreservation.CreateReservationAsync(reservationCreateDto);


            return View(result);

        }

        private RestaurantDbContext CreateContext()
        {          
               return new Restoran.Core.Data.RestoranDbContextFactory().CreateDbContext();
        }
    }
}
