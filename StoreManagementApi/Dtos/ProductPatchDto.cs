namespace StoreManagementApi.Dtos
{
    public class ProductPatchDto
    {
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public int? CategoryId { get; set; }
    }

}
