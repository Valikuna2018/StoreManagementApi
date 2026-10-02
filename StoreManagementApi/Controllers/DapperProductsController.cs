using Dapper;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using StoreManagementApi.Dtos;
using StoreManagementApi.Models;

namespace StoreManagementApi.Controllers
{
    [ApiController]
    [Route("api/dapper-products")]
    public class DapperProductsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public DapperProductsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }




        [HttpGet]
        public async Task<ActionResult> GetAllProducts()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            await using var connection = new NpgsqlConnection(connectionString);

            var sql = """
                SELECT "Id", "Name", "Price", "Quantity", "CategoryId"
                FROM "Products"
                """;

            var products = await connection.QueryAsync<Product>(sql);

            return Ok(products);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult> GetProductById(int id)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            await using var connection = new NpgsqlConnection(connectionString);

            var sql = """
                SELECT "Id", "Name", "Price", "Quantity", "CategoryId"
                FROM "Products"
                WHERE "Id" = @Id
                """;

            var product = await connection.QueryFirstOrDefaultAsync<Product>(
                    sql,
                    new { Id = id }
                );

            if (product is null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult> CreateProduct(CreateProductDto input)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            await using var connection = new NpgsqlConnection(connectionString);

            var sql = """
                INSERT INTO "Products"
                    ("Name", "Price", "Quantity", "CategoryId")
                VALUES
                    (@Name, @Price, @Quantity, @CategoryId)
                RETURNING "Id";
                """;

            var newProductId = await connection.ExecuteScalarAsync<int>(
                    sql,
                   new
                   {
                       Name = input.Name,
                       Price = input.Price,
                       Quantity = input.Quantity,
                       CategoryId = input.CategoryId
                   }
                );

            return Ok(newProductId);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(int id,UpdateProductDto input)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            await using var connection = new NpgsqlConnection(connectionString);

            var sql = """
                UPDATE "Products"
                SET "Name" = @Name,
                    "Price" = @Price,
                    "Quantity" = @Quantity,
                    "CategoryId" = @CategoryId
                WHERE "Id" = @Id;
                """;
            var effectedRows = await connection.ExecuteAsync(sql,
                new
                {
                    Id = id,
                    Name = input.Name,
                    Price = input.Price,
                    Quantity = input.Quantity,
                    CategoryId = input.CategoryId
                });

            return Ok(effectedRows);
            
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            await using var connection = new NpgsqlConnection(connectionString);

            var sql = """
                DELETE FROM "Products"
                WHERE "Id" = @Id
                """;

            var effectedRows = await connection.ExecuteAsync(
                sql,
                new
                {Id = id});

            return Ok(effectedRows);
        }
    }
}