using System.ComponentModel.DataAnnotations;

namespace Task1.Entity
{
    public class Record
    {
        [Required]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Action { get; set; }

        public string Responsibilities { get; set; }

        public DateTime DueDate { get; set; }
    }
}