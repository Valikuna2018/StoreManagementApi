using Microsoft.EntityFrameworkCore;
using StoreManagementApi.Data;
using StoreManagementApi.Models;

namespace StoreManagementApi.Workers
{
    public class LowStockWorker : BackgroundService
    {

        private readonly ILogger<LowStockWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public LowStockWorker(ILogger<LowStockWorker> logger,IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var lowStockProducts = await db.Products
                        .Where(p => p.Quantity < 5)
                        .ToListAsync(stoppingToken);
                    foreach(Product p in lowStockProducts)
                    {
                        _logger.LogWarning($"ProductName : {p.Name}  ProductQuantity : {p.Quantity}");
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(10),stoppingToken);

            }
        }



















        /*
        private readonly ILogger<LowStockWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public LowStockWorker( ILogger<LowStockWorker> logger, IServiceScopeFactory scopeFactory )
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider
                        .GetRequiredService<AppDbContext>();

                    var lowStockProducts = await db.Products
                        .Where(product => product.Quantity < 5)
                        .ToListAsync(stoppingToken);

                    foreach (var product in lowStockProducts)
                    {
                        _logger.LogWarning(
                            "Low stock: {ProductName}, Quantity: {Quantity}",
                            product.Name,
                            product.Quantity);
                    }
                }

                await Task.Delay(
                    TimeSpan.FromSeconds(10),
                    stoppingToken);
            }
        }
        */
    }
}