using Microsoft.EntityFrameworkCore;
using Restoran.Core.Data;
using Restoran.Core.DTOs.Reservation;
using Restoran.Core.Entity;
using Restoran.Core.Statics.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restoran.Core.Business
{
    public class BLLReservation // Sınıf adı BLLReservation olarak değiştirildi
    {
        public BLLReservation()
        {
            // DI ile ilgili hiçbir şey burada olmayacak.
        }

        private RestaurantDbContext CreateContext()
        {
            return new RestoranDbContextFactory().CreateDbContext();
        }

        public async Task<List<ReservationListDto>> GetReservationsAsync()
        {
            using var context = CreateContext();

            var reservations = await context.Reservations
                                        .Include(r => r.Table)
                                        .ToListAsync();

            var reservationListDtos = new List<ReservationListDto>();
            foreach (var reservation in reservations)
            {
                reservationListDtos.Add(new ReservationListDto
                {
                    Id = reservation.Id,
                    CustomerName = reservation.CustomerName,
                    ReservationDateTime = reservation.ReservationDateTime,
                    NumberOfGuests = reservation.NumberOfGuests,
                    Status = reservation.Status,
                    TableNumber = reservation.Table?.TableNumber ?? string.Empty
                });
            }
            return reservationListDtos;
        }

        public async Task<ReservationDetailDto?> GetReservationByIdAsync(int id)
        {
            using var context = CreateContext();

            var reservation = await context.Reservations
                                        .Include(r => r.User)
                                        .Include(r => r.Table)
                                        .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null) return null;

            return new ReservationDetailDto
            {
                Id = reservation.Id,
                CustomerName = reservation.CustomerName,
                CustomerPhone = reservation.CustomerPhone,
                ReservationDateTime = reservation.ReservationDateTime,
                NumberOfGuests = reservation.NumberOfGuests,
                Status = reservation.Status,
                UserId = reservation.UserId,
                Username = reservation.User?.Username,
                TableId = reservation.TableId,
                TableNumber = reservation.Table?.TableNumber ?? string.Empty
            };
        }

        public async Task<(bool Success, string Message)> CreateReservationAsync(ReservationCreateDto dto)
        {
            using var context = CreateContext();

            // 1. Gelecek tarih kontrolü
            if (!IsFutureDate(dto.ReservationDateTime))
                return (false, "Geçmiş tarih için rezervasyon yapılamaz");

            // 2. Çalışma saatleri kontrolü
            if (!IsWorkingHours(dto.ReservationDateTime))
                return (false, "Çalışma saatleri 09:00 - 22:00 arasındadır");

            // 3. Masa müsait mi?
            if (!await IsTableAvailableAsync(dto.TableId, dto.ReservationDateTime))
                return (false, "Bu masa seçilen tarih ve saatte müsait değil");

            // 4. Masa kapasitesi yeterli mi?
            if (!await IsTableCapacityOkAsync(dto.TableId, dto.NumberOfGuests))
                return (false, "Masa kapasitesi kişi sayısı için yeterli değil");

            // Tüm kontroller geçti, rezervasyon oluştur
            var reservation = new Reservation
            {
                CustomerName = dto.CustomerName,
                CustomerPhone = dto.CustomerPhone,
                ReservationDateTime = dto.ReservationDateTime,
                NumberOfGuests = dto.NumberOfGuests,
                UserId = dto.UserId,
                TableId = dto.TableId
            };
            await context.Reservations.AddAsync(reservation);

            var success = await context.SaveChangesAsync() > 0;
            return success ? (true, "Rezervasyon başarıyla oluşturuldu") : (false, "Rezervasyon oluşturulamadı");
        }

        public async Task<bool> UpdateReservationAsync(int id, ReservationUpdateDto dto)
        {
            using var context = CreateContext();

            var reservation = await context.Reservations.FindAsync(id);
            if (reservation == null) return false;

            reservation.CustomerName = dto.CustomerName;
            reservation.CustomerPhone = dto.CustomerPhone;
            reservation.ReservationDateTime = dto.ReservationDateTime;
            reservation.NumberOfGuests = dto.NumberOfGuests;
            reservation.Status = dto.Status;
            reservation.TableId = dto.TableId;

            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteReservationAsync(int id)
        {
            using var context = CreateContext();

            var reservation = await context.Reservations.FindAsync(id);
            if (reservation == null) return false;

            reservation.IsActive = false;

            return await context.SaveChangesAsync() > 0;
        }

        // Validation methodları
        private async Task<bool> IsTableAvailableAsync(int tableId, DateTime reservationDateTime)
        {
            using var context = CreateContext();
            
            // Aynı masa, aynı saatte başka rezervasyon var mı?
            var existingReservation = await context.Reservations
                .FirstOrDefaultAsync(r => r.TableId == tableId && 
                                         r.ReservationDateTime.Date == reservationDateTime.Date &&
                                         r.ReservationDateTime.Hour == reservationDateTime.Hour &&
                                         r.Status != ReservationStatus.Cancelled);
            
            return existingReservation == null;
        }

        private async Task<bool> IsTableCapacityOkAsync(int tableId, int numberOfGuests)
        {
            using var context = CreateContext();
            
            var table = await context.Tables.FindAsync(tableId);
            return table != null && table.Capacity >= numberOfGuests;
        }

        private bool IsWorkingHours(DateTime reservationDateTime)
        {
            var hour = reservationDateTime.Hour;
            return hour >= 9 && hour <= 22; // 09:00 - 22:00 arası
        }

        private bool IsFutureDate(DateTime reservationDateTime)
        {
            return reservationDateTime > DateTime.Now;
        }
    }
} 