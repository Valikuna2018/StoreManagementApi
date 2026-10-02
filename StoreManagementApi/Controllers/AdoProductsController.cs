using Microsoft.AspNetCore.Mvc;
using Npgsql;
using StoreManagementApi.Dtos;
using StoreManagementApi.Models;

namespace StoreManagementApi.Controllers
{
    [ApiController]
    [Route("api/ado-products")]
    public class AdoProductsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AdoProductsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpGet]
        public async Task<ActionResult> GetAllProducts()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            await using var connection = new NpgsqlConnection(connectionString);

            await connection.OpenAsync();

            var sql = """ 
                SELECT "Id", "Name", "Price", "Quantity", "CategoryId"
                FROM "Products"
                """;

            await using var command = new NpgsqlCommand(sql, connection);

            await using var reader =  await command.ExecuteReaderAsync();

            var products = new List<Product>();

            while (await reader.ReadAsync())
            {
                var product = new Product
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Price = reader.GetDecimal(2),
                    Quantity = reader.GetInt32(3),
                    CategoryId = reader.GetInt32(4)
                };

                products.Add(product);
            }

            return Ok(products);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult> GetProductById(int id)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            await using var connection = new NpgsqlConnection(connectionString);

            await connection.OpenAsync();

            var sql = """
                SELECT "Id", "Name", "Price", "Quantity", "CategoryId"
                FROM "Products"
                WHERE "Id" = @Id
                """;

            await using var command = new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Id", id);

            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return NotFound();

            var product = new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Price = reader.GetDecimal(2),
                Quantity = reader.GetInt32(3),
                CategoryId = reader.GetInt32(4)
            };

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult> CreateProduct(CreateProductDto input)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            await using var connection = new NpgsqlConnection(connectionString);

            await connection.OpenAsync();

            var sql = """
                INSERT INTO "Products"
                    ("Name", "Price", "Quantity", "CategoryId")
                VALUES
                    (@Name, @Price, @Quantity, @CategoryId)
                RETURNING "Id";
                """;

            await using var command = new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Name", input.Name);
            command.Parameters.AddWithValue("@Price", input.Price);
            command.Parameters.AddWithValue("@Quantity", input.Quantity);
            command.Parameters.AddWithValue("@CategoryId", input.CategoryId);

            var newProductId = (int)(await command.ExecuteScalarAsync())!;

            return Ok(newProductId);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(int id, UpdateProductDto input)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            await using var connection = new NpgsqlConnection(connectionString);

            await connection.OpenAsync();

            var sql = """
                UPDATE "Products"
                SET "Name" = @Name,
                    "Price" = @Price,
                    "Quantity" = @Quantity,
                    "CategoryId" = @CategoryId
                WHERE "Id" = @Id;
                """;

            await using var command = new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Name", input.Name);
            command.Parameters.AddWithValue("@Price", input.Price);
            command.Parameters.AddWithValue("@Quantity", input.Quantity);
            command.Parameters.AddWithValue("@CategoryId", input.CategoryId);

            var affectedRows = await command.ExecuteNonQueryAsync();

            if (affectedRows == 0)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            await using var connection = new NpgsqlConnection(connectionString);

            await connection.OpenAsync();

            var sql = """
                DELETE FROM "Products"
                WHERE "Id" = @Id
                """;

            await using var command = new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Id", id);

            int effectedRows = await command.ExecuteNonQueryAsync();

            return Ok(effectedRows);
        }
    }
}