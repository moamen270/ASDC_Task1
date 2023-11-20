using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Task1.Data;
using Task1.Entity;
using Task1.Entity.DTO;

namespace Task1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /records
        [HttpGet("records")]
        public IActionResult GetRecords()
        {
            var records = _context.Records.AsNoTracking().ToList();
            return Ok(records);
        }

        // GET /record/{id}
        [HttpGet("record/{id}")]
        public IActionResult GetRecord(int id)
        {
            var record = _context.Records.Find(id);
            if (record == null) return NotFound();
            return Ok(record);
        }

        // POST /record
        [HttpPost("record")]
        public IActionResult CreateRecord(RecordDto recordDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var record = new Record
            {
                Name = recordDto.RecordName,
                Action = recordDto.RecordAction,
                Responsibilities = recordDto.RecordResponsibilities,
                DueDate = recordDto.RecordDueDate
            };
            _context.Records.Add(record);
            _context.SaveChanges();
            return Ok(record);
        }

        // PUT /record/{id}
        [HttpPut("record")]
        public IActionResult UpdateRecord(Record record)
        {
            if (ModelState.IsValid) return BadRequest(ModelState);

            if (!_context.Records.Any(r => r.Id == record.Id)) return BadRequest();

            _context.Update(record);
            _context.SaveChanges();

            return NoContent();
        }

        // DELETE /record/{id}
        [HttpDelete("record/{id}")]
        public IActionResult DeleteRecord(int id)
        {
            var record = _context.Records.Find(id);
            if (record == null) return NotFound();

            _context.Records.Remove(record);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPost("import")]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest();

            if (!Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Only CSV files allowed");

            using var reader = new StreamReader(file.OpenReadStream());
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<Record>().ToList();
            records.ForEach(r => r.Id = 0);
            _context.Records.AddRange(records);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}