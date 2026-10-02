using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreManagementApi.Data;
using StoreManagementApi.Dtos;
using StoreManagementApi.Models;

namespace StoreManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CategoriesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<List<Category>>> GetAllCategories()
        {
            var categories = await _db.Categories.ToListAsync();

            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategoryById(int id)
        {
            var category = await _db.Categories.FindAsync(id);

            if (category is null)
                return NotFound();

            return Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult> CreateCategory(CreateCategoryDto input)
        {
            var category = new Category { Name = input.Name };

            await _db.Categories.AddAsync(category);
            await _db.SaveChangesAsync();

            return Ok("Category created");
        }

        [HttpGet("products-by-category/{id}")]
        public async Task<ActionResult> GetProductsByCategory(int id)
        {
            var category = await _db.Categories.FindAsync(id);

            if (category is null)
                return NotFound();

            await _db.Entry(category)
                .Collection(category => category.Products)
                .LoadAsync();

            var products = category.Products
                .Select(product => new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = product.Quantity,
                    CategoryId = product.CategoryId
                }).ToList();

            return Ok(products);
        }

    }
}
