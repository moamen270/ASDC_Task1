using System.ComponentModel;

namespace Task1.Entity.DTO
{
    public record RecordDto
    {
        public string RecordName { get; set; }
        public string RecordAction { get; set; }

        public string RecordResponsibilities { get; set; }

        public DateTime RecordDueDate { get; set; }
    }
}