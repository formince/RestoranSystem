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

        // Ürün bilgilerini database'den al
        var productIds = dto.Items.Select(x => x.ProductId).ToList();
        var products = await context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();

        // Validation - ürün kontrolü
        foreach (var item in dto.Items)
        {
            var product = products.FirstOrDefault(p => p.Id == item.ProductId);
            if (product == null)
                return (false, $"Ürün bulunamadı (ID: {item.ProductId})");

            if (!product.IsActive)
                return (false, $"'{product.Name}' ürünü aktif değil");
                
            if (product.StockQuantity < item.Quantity)
                return (false, $"'{product.Name}' ürünü için yeterli stok yok (Mevcut: {product.StockQuantity})");
        }

        // Toplam tutarı hesapla (database'deki güncel fiyatlarla)
        decimal totalAmount = 0;
        foreach (var item in dto.Items)
        {
            var product = products.First(p => p.Id == item.ProductId);
            totalAmount += product.Price * item.Quantity;
        }

        if (!IsMinimumOrderAmountOk(totalAmount))
            return (false, "Minimum sipariş tutarı 50 TL olmalıdır");

        try
        {
            // Sipariş oluştur
            var order = new Order
            {
                UserId = dto.UserId,
                OrderDate = DateTime.Now,
                OrderNumber = GenerateOrderNumber(),
                TotalAmount = totalAmount,
                Status = Statics.Enums.OrderStatus.Pending
            };

            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();

            // Sipariş detaylarını ekle ve stokları güncelle
            foreach (var item in dto.Items)
            {
                var product = products.First(p => p.Id == item.ProductId);
                
                // Stok azalt
                product.StockQuantity -= item.Quantity;

                // Sipariş detayı ekle
                var orderDetail = new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price // Database'deki güncel fiyatı kullan
                };
                await context.OrderDetails.AddAsync(orderDetail);
            }

            var success = await context.SaveChangesAsync() > 0;
            return success ? (true, "Sipariş başarıyla oluşturuldu") : (false, "Sipariş oluşturulamadı");
        }
        catch (Exception ex)
        {
            return (false, $"Sipariş oluşturulurken hata: {ex.Message}");
        }
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



    private bool IsMinimumOrderAmountOk(decimal totalAmount)
    {
        return totalAmount >= 50; // Minimum 50 TL
    }
}
