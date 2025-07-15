using Microsoft.EntityFrameworkCore;
using Restoran.Core.Data;
using Restoran.Core.DTOs.Order;
using Restoran.Core.Entity;

namespace Restoran.Core.Business;

public class BLLOrder 
{
    public BLLOrder()
    {
        
    }

    private RestaurantDbContext CreateContext()
    {
        return new RestoranDbContextFactory().CreateDbContext();
    }

    public async Task<List<OrderListDto>> GetOrdersAsync()
    {
        using var context = CreateContext();

        var orders = await context.Orders
                                    .Include(o => o.User)
                                    .ToListAsync();

        var orderListDtos = new List<OrderListDto>();
        foreach (var order in orders)
        {
            orderListDtos.Add(new OrderListDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                CustomerUsername = order.User?.Username
            });
        }
        return orderListDtos;
    }

    public async Task<OrderDetailDto?> GetOrderByIdAsync(int id)
    {
        using var context = CreateContext();

        var order = await context.Orders
                                    .Include(o => o.User!)
                                    .Include(o => o.OrderDetails!)
                                    .ThenInclude(od => od.Product)
                                    .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return null;

        var orderItems = new List<OrderItemDto>();
        foreach (var detail in order.OrderDetails!)
        {
            orderItems.Add(new OrderItemDto
            {
                ProductId = detail.ProductId,
                ProductName = detail.Product?.Name!,
                Quantity = detail.Quantity,
                UnitPrice = detail.UnitPrice
            });
        }

        return new OrderDetailDto
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            CustomerUsername = order.User?.Username,
            Items = orderItems
        };
    }

    public async Task<(bool Success, string Message)> CreateOrderAsync(OrderCreateDto dto)
    {
        using var context = CreateContext();

       
        if (dto.Items == null || dto.Items.Count == 0)
            return (false, "Sipariş boş olamaz");

       
        foreach (var item in dto.Items)
        {
            if (!await IsProductActiveAsync(item.ProductId))
                return (false, $"'{item.ProductName}' ürünü aktif değil");
                
            if (!await IsStockAvailableAsync(item.ProductId, item.Quantity))
                return (false, $"'{item.ProductName}' ürünü için yeterli stok yok");
        }

       
        var totalAmount = CalculateTotalAmount(dto.Items);
        if (!IsMinimumOrderAmountOk(totalAmount))
            return (false, "Minimum sipariş tutarı 50 TL olmalıdır");

        
        var order = new Order
        {
            UserId = dto.UserId,
            OrderDate = DateTime.Now,
            OrderNumber = GenerateOrderNumber(),
            TotalAmount = totalAmount
        };

        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();

        
        foreach (var item in dto.Items)
        {
            // Stok azalt
            var product = await context.Products.FindAsync(item.ProductId);
            if (product != null)
            {
                product.StockQuantity -= item.Quantity;
            }

            // Sipariş detayı ekle
            var orderDetail = new OrderDetail
            {
                OrderId = order.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            };
            await context.OrderDetails.AddAsync(orderDetail);
        }

        var success = await context.SaveChangesAsync() > 0;
        return success ? (true, "Sipariş başarıyla oluşturuldu") : (false, "Sipariş oluşturulamadı");
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        using var context = CreateContext();

        var order = await context.Orders.FindAsync(id);
        if (order == null) return false;
        order.Status=Statics.Enums.OrderStatus.Cancelled;
        order.IsActive = false;

        return await context.SaveChangesAsync() > 0;
    }

    private string GenerateOrderNumber()
    {
        return $"ORD{DateTime.Now:yyyyMMddHHmmss}";
    }

    private decimal CalculateTotalAmount(List<OrderItemDto> items)
    {
        return items.Sum(item => item.Quantity * item.UnitPrice);
    }

    // Validation methodları
    private async Task<bool> IsStockAvailableAsync(int productId, int quantity)
    {
        using var context = CreateContext();
        
        var product = await context.Products.FindAsync(productId);
        return product != null && product.StockQuantity >= quantity;
    }

    private async Task<bool> IsProductActiveAsync(int productId)
    {
        using var context = CreateContext();
        
        var product = await context.Products.FindAsync(productId);
        return product != null && product.IsActive;
    }

    private bool IsMinimumOrderAmountOk(decimal totalAmount)
    {
        return totalAmount >= 50; // Minimum 50 TL
    }
}
