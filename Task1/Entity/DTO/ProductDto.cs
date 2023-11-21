using System.ComponentModel;

namespace Task1.Entity.DTO
{
    public record ProductDto
    {
        public string ProductName { get; set; }

        public string ProductDescription { get; set; }

        public string LocationFind { get; set; }

        public decimal Price { get; set; }

        public string Color { get; set; }
    }
}