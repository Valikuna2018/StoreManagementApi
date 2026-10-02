using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using StoreManagementApi.Data;
using StoreManagementApi.Dtos;
using StoreManagementApi.Models;

namespace StoreManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ProductsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAllProduct()
        {
            var products = await _db.Products.ToListAsync();

            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            var product = await _db.Products.FindAsync(id);

            if (product is null)
                return NotFound();

            return Ok(product);
        }

        [HttpGet("category-by-id/{id}")]
        public async Task<ActionResult<string>> GetCategoryById(int id)
        {
            var product = await _db.Products.FindAsync(id);

            if (product is null)
                return NotFound();

            await _db.Entry(product)
                .Reference(product => product.Category)
                .LoadAsync();

            return Ok(product.Category.Name);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(CreateProductDto input)
        {

            bool categpryExists = await _db.Categories.AnyAsync(c => c.Id == input.CategoryId);

            if (!categpryExists) return NotFound("there is not any category");

            var product = new Product
            {
                Name = input.Name,
                Price = input.Price,
                Quantity = input.Quantity,
                CategoryId = input.CategoryId
            };


            await _db.Products.AddAsync(product);
            await _db.SaveChangesAsync();

            return Ok(product);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(int id, UpdateProductDto updatedProduct)
        {
            var product = await _db.Products.FindAsync(id);

            if (product is null)
                return NotFound();

            product.Name = updatedProduct.Name;
            product.Price = updatedProduct.Price;
            product.Quantity = updatedProduct.Quantity;
            product.CategoryId = updatedProduct.CategoryId;

            await _db.SaveChangesAsync();
            return Ok();
        }


        [HttpPatch("{id}")]
        public async Task<ActionResult> PatchProduct(int id, ProductPatchDto patch)
        {
            var product = await _db.Products.FindAsync(id);

            if (product is null)
                return NotFound();

            if (patch.Name is not null)
                product.Name = patch.Name;

            if (patch.Price.HasValue)
                product.Price = patch.Price.Value;

            if (patch.Quantity.HasValue)
                product.Quantity = patch.Quantity.Value;

            if (patch.CategoryId.HasValue)
                product.CategoryId = patch.CategoryId.Value;

            await _db.SaveChangesAsync();

            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _db.Products.FindAsync(id);

            if (product is null)
                return NotFound();

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("low-stock")]
        public async Task<ActionResult<List<Product>>> GetLowStockProducts()
        {
            var products = await _db.Products
                .Where(p => p.Quantity < 5)
                .ToListAsync();

            return Ok(products);
        }
        [HttpGet("by-price")]
        public async Task<ActionResult<List<Product>>> GetProductsByPrice()
        {
            var products = await _db.Products
                .OrderByDescending(p => p.Price)
                .ToListAsync();

            return Ok(products);
        }


        [HttpGet("most-expensive")]
        public async Task<ActionResult<Product>> GetMostExpensiveProduct()
        {
            var product = await _db.Products
                .OrderByDescending(p => p.Price)
                .FirstOrDefaultAsync();

            if (product is null)
                return NotFound("product not found");
            return Ok(product);
        }

        [HttpGet("low-stock-count")]
        public async Task<ActionResult<int>> GetLowStockCount()
        {
            int count = await _db.Products
                .CountAsync(p => p.Quantity < 5);

            return count;
        }

        [HttpGet("average-price")]
        public async Task<ActionResult<decimal>> GetAveragePrice()
        {
            decimal avPrice = await _db.Products
                .AverageAsync(p => p.Price);

            return avPrice;
        }


        [HttpGet("group-by-category")]
        public async Task<ActionResult> GetProductsGroupedByCategory()
        {
            var result = await _db.Products
                .GroupBy(p => p.CategoryId)
                .Select(group => new
                {
                    CategoryId = group.Key,
                    ProductCount = group.Count()
                })
                .ToListAsync();

            return Ok(result);
        }
        [HttpGet("average-price-by-category")]
        public async Task<ActionResult> GetAveragePriceByCategory()
        {
            var result = await _db.Products
                .GroupBy(p => p.CategoryId)
                .Select(group => new
                {
                    CategoryId = group.Key,
                    AveragePrice = group.Average(g => g.Price)

                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("products-with-categories")]
        public async Task<ActionResult> GetProductsWithCategories()
        {
            var result = await _db.Products
                .Join(_db.Categories,
                product => product.CategoryId,
                category => category.Id,
                (product, category) => new
                {
                    ProductName = product.Name,
                    ProductPrice = product.Price,
                    CategoryName = category.Name
                }).ToListAsync();

            return Ok(result);
        }

        [HttpGet("has-expensive-products")]
        public async Task<ActionResult<bool>> HasExpensiveProducts()
        {
            var hasExpensiveProducts = await _db.Products.AnyAsync(p => p.Price > 1000);

            return hasExpensiveProducts;
        }

        [HttpGet("all-in-stock")]
        public async Task<ActionResult<bool>> AreAllProductsInStock()
        {
            var allProductsInStock = await _db.Products.AllAsync(p => p.Quantity > 0);
            return allProductsInStock;
        }
    }
}
