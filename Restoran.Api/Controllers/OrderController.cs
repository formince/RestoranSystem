using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restoran.Core.Business;
using Restoran.Core.DTOs.Order;

namespace Restoran.Api.Controllers
{
    public class OrderController : BaseController
    {
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetOrders()
        {
            var orderBLL = new BLLOrder();
            var orders = await orderBLL.GetOrdersAsync();
            return HandleResult(orders, "Siparişler başarıyla getirildi");
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "CustomerOrAdmin")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var orderBLL = new BLLOrder();
            var order = await orderBLL.GetOrderByIdAsync(id);
            return HandleResult(order, "Sipariş başarıyla getirildi");
        }

        [HttpPost]
        [Authorize(Policy = "CustomerOrAdmin")]
        public async Task<IActionResult> CreateOrder(OrderCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var orderBLL = new BLLOrder();
            var result = await orderBLL.CreateOrderAsync(dto);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var orderBLL = new BLLOrder();
            var result = await orderBLL.DeleteOrderAsync(id);
            return HandleResult(result, result ? "Sipariş başarıyla silindi" : "Sipariş silinemedi");
        }
    }
} 