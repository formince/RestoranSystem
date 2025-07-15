using Microsoft.EntityFrameworkCore;
using Restoran.Core.Data;
using Restoran.Core.DTOs.Table;
using Restoran.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restoran.Core.Business
{
    public class BLLTable 
    {
        public BLLTable()
        {
           
        }

        private RestaurantDbContext CreateContext()
        {
            return new RestoranDbContextFactory().CreateDbContext();
        }

        public async Task<List<TableListDto>> GetTablesAsync()
        {
            using var context = CreateContext();

            var tables = await context.Tables.ToListAsync();

            var tableListDtos = new List<TableListDto>();
            foreach (var table in tables)
            {
                tableListDtos.Add(new TableListDto
                {
                    Id = table.Id,
                    TableNumber = table.TableNumber,
                    Capacity = table.Capacity,
                    IsAvailable = table.IsAvailable
                });
            }
            return tableListDtos;
        }

        public async Task<TableDetailDto?> GetTableByIdAsync(int id)
        {
            using var context = CreateContext();

            var table = await context.Tables.FirstOrDefaultAsync(t => t.Id == id);

            if (table == null) return null;

            return new TableDetailDto
            {
                Id = table.Id,
                TableNumber = table.TableNumber,
                Capacity = table.Capacity,
                IsAvailable = table.IsAvailable
            };
        }

        public async Task<(bool Success, string Message)> CreateTableAsync(TableCreateDto dto)
        {
            using var context = CreateContext();

           
            if (!await IsTableNumberUniqueAsync(dto.TableNumber))
                return (false, $"'{dto.TableNumber}' masa numarası zaten kullanılıyor");

           
            if (dto.Capacity <= 0)
                return (false, "Masa kapasitesi 0'dan büyük olmalıdır");

            var table = new Table
            {
                TableNumber = dto.TableNumber,
                Capacity = dto.Capacity,
                IsAvailable = dto.IsAvailable
            };
            await context.Tables.AddAsync(table);

            var success = await context.SaveChangesAsync() > 0;
            return success ? (true, "Masa başarıyla oluşturuldu") : (false, "Masa oluşturulamadı");
        }

        public async Task<(bool Success, string Message)> UpdateTableAsync(int id, TableUpdateDto dto)
        {
            using var context = CreateContext();

            var table = await context.Tables.FindAsync(id);
            if (table == null) return (false, "Masa bulunamadı");

           
            if (!await IsTableNumberUniqueAsync(dto.TableNumber, id))
                return (false, $"'{dto.TableNumber}' masa numarası zaten kullanılıyor");

           
            if (dto.Capacity <= 0)
                return (false, "Masa kapasitesi 0'dan büyük olmalıdır");

            table.TableNumber = dto.TableNumber;
            table.Capacity = dto.Capacity;
            table.IsAvailable = dto.IsAvailable;

            var success = await context.SaveChangesAsync() > 0;
            return success ? (true, "Masa başarıyla güncellendi") : (false, "Masa güncellenemedi");
        }

        public async Task<bool> DeleteTableAsync(int id)
        {
            using var context = CreateContext();

            var table = await context.Tables.FindAsync(id);
            if (table == null) return false;

            table.IsActive = false;

            return await context.SaveChangesAsync() > 0;
        }

       
        private async Task<bool> IsTableNumberUniqueAsync(string tableNumber, int? excludeId = null)
        {
            using var context = CreateContext();
            
            var existingTable = await context.Tables
                .FirstOrDefaultAsync(t => t.TableNumber == tableNumber && 
                                         (excludeId == null || t.Id != excludeId));
            
            return existingTable == null;
        }
    }
} 