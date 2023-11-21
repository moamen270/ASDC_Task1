using System.ComponentModel;

namespace Task1.Entity.DTO
{
    public record ProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public decimal Price { get; set; }
        public string Color { get; set; }
    }
}