namespace StoreManagementApi.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

    }
}
