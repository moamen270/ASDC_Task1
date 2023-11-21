using System.ComponentModel.DataAnnotations;

namespace Task1.Entity
{
    public class Product
    {
        [Required]
        public int ID { get; set; }

        public string ProductName { get; set; }

        public string ProductDescription { get; set; }

        public string LocationFind { get; set; }

        public decimal Price { get; set; }

        public string Color { get; set; }
    }
}