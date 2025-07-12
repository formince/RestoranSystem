using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restoran.Core.Business;
using Restoran.Core.DTOs.Reservation;

namespace Restoran.Api.Controllers
{
    public class ReservationController : BaseController
    {
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetReservations()
        {
            var reservationBLL = new BLLReservation();
            var reservations = await reservationBLL.GetReservationsAsync();
            return HandleResult(reservations, "Rezervasyonlar başarıyla getirildi");
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "CustomerOrAdmin")]
        public async Task<IActionResult> GetReservation(int id)
        {
            var reservationBLL = new BLLReservation();
            var reservation = await reservationBLL.GetReservationByIdAsync(id);
            return HandleResult(reservation, "Rezervasyon başarıyla getirildi");
        }

        [HttpPost]
        [Authorize(Policy = "CustomerOrAdmin")]
        public async Task<IActionResult> CreateReservation(ReservationCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reservationBLL = new BLLReservation();
            var result = await reservationBLL.CreateReservationAsync(dto);
            return HandleResult(result);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateReservation(int id, ReservationUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reservationBLL = new BLLReservation();
            var result = await reservationBLL.UpdateReservationAsync(id, dto);
            return HandleResult(result, result ? "Rezervasyon başarıyla güncellendi" : "Rezervasyon güncellenemedi");
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservationBLL = new BLLReservation();
            var result = await reservationBLL.DeleteReservationAsync(id);
            return HandleResult(result, result ? "Rezervasyon başarıyla silindi" : "Rezervasyon silinemedi");
        }
    }
} 