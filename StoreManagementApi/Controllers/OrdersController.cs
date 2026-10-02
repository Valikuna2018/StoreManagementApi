using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreManagementApi.Data;
using StoreManagementApi.Dtos;
using StoreManagementApi.Models;

namespace StoreManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _db;

        public OrdersController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderDto>>> GetAllOrders()
        {
            var orders = await _db.Orders
                .Include(o => o.Product)
                .ThenInclude(p => p.Category)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    Quantity = o.Quantity,
                    CreatedAt = o.CreatedAt,

                    ProductId = o.ProductId,
                    ProductName = o.Product.Name,

                    CategoryId = o.Product.CategoryId,
                    CategoryName = o.Product.Category.Name
                })
                .ToListAsync();

            return Ok(orders);
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrder(CreateOrderDto input)
        {
            bool productExists = await _db.Products.AnyAsync(p => p.Id == input.ProductId);
            
            if (!productExists) return NotFound("Product not found");

            var order = new Order
            {
                ProductId = input.ProductId,
                Quantity = input.Quantity,
                CreatedAt = DateTime.UtcNow
            };

            await _db.Orders.AddAsync(order);
            await _db.SaveChangesAsync();
            return Ok(order);
        }
    }
}
