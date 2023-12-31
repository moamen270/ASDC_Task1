﻿using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Task1.Data;
using Task1.Entity;
using Task1.Entity.DTO;
using ExcelDataReader;
using OfficeOpenXml;

namespace Task1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /products
        [HttpGet("products")]
        public IActionResult GetRecords()
        {
            var products = _context.Products.AsNoTracking().ToList();
            return Ok(products);
        }

        // GET /product/{id}
        [HttpGet("product/{id}")]
        public IActionResult GetRecord(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        // POST /product
        [HttpPost("product")]
        public IActionResult CreateRecord(ProductDto productDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Name,
                Location = productDto.Location,
                Price = productDto.Price,
                Color = productDto.Color
            };
            _context.Products.Add(product);
            _context.SaveChanges();
            return Ok(product);
        }

        // PUT /product/{id}
        [HttpPut("product")]
        public IActionResult UpdateRecord(Product product)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!_context.Products.Any(r => r.ID == product.ID)) return BadRequest();

            _context.Update(product);
            _context.SaveChanges();

            return Ok(product);
        }

        // DELETE /product/{id}
        [HttpDelete("product/{id}")]
        public IActionResult DeleteRecord(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPost("import")]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest();

            var products = new List<Product>();
            try
            {
                if (Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    using (var reader = new StreamReader(file.OpenReadStream()))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        // Assuming the order of columns in the CSV matches the order in the Product class
                        csv.Read();
                        csv.ReadHeader();

                        while (csv.Read())
                        {
                            var product = new Product
                            {
                                ID = int.Parse(csv.GetField(0)),
                                Name = csv.GetField(1),
                                Description = csv.GetField(2),
                                Location = csv.GetField(3),
                                Price = decimal.Parse(csv.GetField(4)),
                                Color = csv.GetField(5)
                            };

                            products.Add(product);
                        }
                    }
                    products.ForEach(r => r.ID = 0);
                    _context.Products.AddRange(products);
                    await _context.SaveChangesAsync();
                    return Ok(products);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            try
            {
                if (file.FileName.EndsWith(".xlsx"))
                {
                    products = ReadExcelFile(file);
                    products.ForEach(r => r.ID = 0);
                    // Save the list of products to the database
                    _context.Products.AddRange(products);
                    _context.SaveChanges();

                    return Ok(products);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return BadRequest("Only .csv & .xlsx files allowed");
        }

        private List<Product> ReadExcelFile(IFormFile file)
        {
            List<Product> products = new List<Product>();

            using (var package = new ExcelPackage(file.OpenReadStream()))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Assuming the data is in the first sheet

                for (int row = 2; row <= worksheet.Dimension.End.Row; row++) // Assuming the header is in the first row
                {
                    var product = new Product
                    {
                        ID = int.Parse(worksheet.Cells[row, 1].Value.ToString()),
                        Name = worksheet.Cells[row, 2].Value.ToString(),
                        Description = worksheet.Cells[row, 3].Value.ToString(),
                        Location = worksheet.Cells[row, 4].Value.ToString(),
                        Price = decimal.Parse(worksheet.Cells[row, 5].Value.ToString()),
                        Color = worksheet.Cells[row, 6].Value.ToString()
                    };

                    products.Add(product);
                }
            }

            return products;
        }
    }
}