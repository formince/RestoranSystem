using Restoran.Core.DTOs.Order;
using Restoran.Core.Statics.Enums;

namespace Restoran.Core.Business.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderListDto>> GetAllAsync();
        Task<OrderDetailDto?> GetByIdAsync(int id);
        Task<bool> CreateAsync(OrderCreateDto dto);
        Task<bool> UpdateStatusAsync(int id, OrderStatus status);
        Task<bool> DeleteAsync(int id);
        
        // Order'a özel methodlar
        Task<List<OrderListDto>> GetOrdersByUserAsync(int userId);
        Task<List<OrderListDto>> GetOrdersByStatusAsync(OrderStatus status);
        Task<string> GenerateOrderNumberAsync();
    }
}