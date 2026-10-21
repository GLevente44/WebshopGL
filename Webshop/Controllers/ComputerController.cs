using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI.Common;
using Webshop.Models;
using Webshop.Models.Dtos;

namespace Webshop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComputerController : ControllerBase
    {
        public ComputerShopDbContext context = new ComputerShopDbContext();

        [HttpGet("getAll")]
        public object GetAllComputer()
        {

            var computers = context.Computers.ToList();
            if (computers.Count > 0)
            {
                return new { message = "Sikeres lekérdezés", result = "" };
            }
            return StatusCode(201, new { message = "Sikeres felvétel", result = computers });
        }

        [HttpPost]
        public object AddNewComputer(AddNewComputerDto addNewComputerDto)
        {
            var computer = new Computer
            {
                Id = Guid.NewGuid(),
                Brand = addNewComputerDto.Brand,
                Type = addNewComputerDto.Type,
                Display = addNewComputerDto.Display,
                Memory = addNewComputerDto.Memory,
                CreatedTime = DateTime.Now,
                Osys = addNewComputerDto.Osys,
            };

            context.Computers.Add(computer);
            context.SaveChanges();

            return StatusCode(201, new { message = "Sikeres felvétel", result = computer });
        }

        [HttpPut]
        public object UpdateComputer([FromQuery] Guid id, [FromBody] UpdateComputer updateComputerDto)
        {
            var computer = context.Computers.FirstOrDefault(computer => computer.Id == id);

            if (computer != null)
            {
                computer.Brand = updateComputerDto.Brand;
                computer.Type = updateComputerDto.Type;
                computer.Display = updateComputerDto.Display;
                computer.Memory = updateComputerDto.Memory;


                context.Computers.Add(computer);
                context.SaveChanges();

                return StatusCode(200, new { message = "Sikeres frissítés", result = computer });

            }

            return StatusCode(404, new { message = "Sikertelen felvétel", result = computer });
        }

        [HttpDelete]
        public object DeleteComputer([FromQuery] Guid id)
        {
            var computer = context.Computers.Find(id);

            if (computer != null)
            {
                context.Computers.Remove(computer);
                context.SaveChanges();

                return StatusCode(204, new { message = "Sikeres törlés" });
            }

            return StatusCode(404, new { message = "Sikertelen törlés" });
        }

        [HttpGet("byId")]
        public object GetById([FromQuery] Guid id)
        {
            var computer = context.Computers.SingleOrDefault(x => x.Id == id);

            if (computer != null)
            {
                return StatusCode(200, new { message = "van találat", result = computer });

            }
            return StatusCode(404, new { message = "nincs találat", result = computer });
        }

        // 1.
        [HttpGet("getOsystemAndComputer")]
        public object GetOsystemAndComputer()
        {
            var result = context.Osystems.Include(os => os.Computers).ToList();
            if (result.Count > 0)
            {
                return new { message = "Sikeres lekérdezés", result = "" };

            }
            return StatusCode(404, new { message = "üres adatbázis", result = result });

        }
        // 2. Feladat
        [HttpGet("getAllComputerbyType")]
        public object GetAllComputerbyType()
        {
            var result = context.Osystems.Include(os => os.Computers).ToList();
            if (result.Count > 0)
            {
                return new { message = "Sikeres lekérdezés", result = "" };

            }
            return StatusCode(404, new { message = "üres adatbázis", result = result });
        }

        // 3. Feladat
        [HttpGet("~/api/query/os/{id:guid}")]
        public object GetOsById(Guid id)
        {
            var result = context.Osystems.Include(os => os.Computers).FirstOrDefault(os => os.Id == id);

            if (result == null)
            {
                return NotFound(new { message = "Nincs ilyen ope rendszer" });

            }

            return Ok(new { message = "Sikeres lekérdezés", result = result });
        }

        // 4. Feladat
        [HttpGet("onOsystemOneComputer")]
        public object OnOsystemOneComputer([FromQuery] Guid id)
        {
            var computer = context.Computers.Where(c => c.Id == id).Select(c => new { OsName = c.Osys.Name }).FirstOrDefault();

            if (computer == null)
            {
                return NotFound(new { message = "Nincs ilyen számítógép" });
            }
            return "Sikeres lekérdezés";
        }

        // 5.Feladat

        [HttpGet("GetComputerWithOs")]
        public object GetComputerWithOs([FromQuery]Guid id)
        {
            var os = context.Osystems.Include(os => os.Computers).FirstOrDefault(os => os.Computers.Any(c => c.Id == id));

            if (os == null)
            {
                return new { message = "Nincs ilyen számítógép" };

            }

            var computer = os.Computers.First(c => c.Id == id);

            return new {message = "Sikeres lekérdezés",result = new{Computer = computer,OsName = os.Name}};
        }

        // 6.Feladat
        [HttpGet("GetOsComputerCount")]
        public object GetOsComputerCount([FromQuery] Guid id)
        {
            var result = context.Osystems.Select(os => new { OsName = os.Name, ComputerCount = os.Computers.Count() }).ToList();

            return new { message = "Sikeres lekérdezés", result };
        }

        //7. Feladat
        [HttpGet("GetComputersSorted")]
        public object GetComputersSorted([FromQuery] Guid id)
        {
            var result = context.Osystems.SelectMany(os => os.Computers.Select(c => new{ ComputerId = c.Id, Brand = c.Brand, Type = c.Type, OsName = os.Name })).OrderBy(c => c.Brand).ThenBy(c => c.Type).ToList();

            return new { message = "Sikeres lekérdezés", result };
        }

        // 8.
        [HttpGet("GetMinDisplay")]
        public object GetMinDisplay(double size)
        {
            var result = context.Osystems.SelectMany(os => os.Computers.Where(c => c.Display > size).Select(c => new { c.Id, c.Brand, c.Type, c.Display, OsName = os.Name })).ToList();

            return new { message = "Sikeres lekérdezés", result };

        }

        // 9.
        [HttpGet("GetOsStats")]
        public object GetOsStats([FromQuery] Guid id)
        {
            var result = context.Osystems.Select(os => new{OsName = os.Name,ComputerCount = os.Computers.Count(),AverageMemory = os.Computers.Select(c => (double)c.Memory).Average(),MaxMemory = os.Computers.Select(c => (int)c.Memory).Max()}).ToList();

            return new { message = "Sikeres lekérdezés", result }; 
        }

        // 10.
        [HttpGet("GetComputersByOs")]
        public object GetComputersByOs([FromQuery] string name)
        {
            var result = context.Osystems.Where(os => os.Name == name).SelectMany(os => os.Computers.Select(c => new {c.Id,c.Brand,c.Type,c.Display,c.Memory,OsName = os.Name})).ToList();

            if (result.Count == 0)
            {
                return new { message = "Nincs ilyen operációs rendszer vagy gép" };
            }

            return new { message = "Sikeres lekérdezés", result };
        }

        // 11.
        [HttpGet("GetOsWithoutComputer")]
        public object GetOsWithoutComputers()
        {
            var result = context.Osystems.Where(os => !os.Computers.Any()) .Select(os => new{os.Id,OsName = os.Name }).ToList();

            return new { message = "Sikeres lekérdezés", result };
        }

        // 12.
        [HttpGet("GetLargestMemoryByOs")]
        public object GetLargestMemoryByOs()
        {
            var result = context.Osystems.Where(os => os.Computers.Any()).Select(os => new{OsName = os.Name,Computer = os.Computers.OrderByDescending(c => c.Memory).Select(c => new{c.Id,c.Brand,c.Type,c.Memory}).FirstOrDefault()}).ToList();

            return new { message = "Sikeres lekérdezés", result };
        }

        // 13.
        [HttpGet("FilterComputer")]
        public object FilterComputers([FromQuery] int minMemory,[FromQuery] string osName)
        {
            var result = context.Osystems.Where(os => os.Name.Contains(osName)) .SelectMany(os => os.Computers .Where(c => c.Memory >= minMemory) .Select(c => new{c.Id,c.Brand,c.Type, c.Memory,OsName = os.Name})).ToList();

            return new { message = "Sikeres lekérdezés", result };
        }

        // 14.
        [HttpGet("GetLatestComputers")]
        public object GetLatestComputers()
        {var result = context.Osystems.SelectMany(os => os.Computers.Select(c => new{c.Id, c.Brand,c.Type,c.CreatedTime,OsName = os.Name })).OrderByDescending(c => c.CreatedTime).Take(5).ToList();

            return new { message = "Sikeres lekérdezés", result };
        }

        // 15. 
        [HttpGet("GetBrandSummary")]
        public object GetBrandSummary()
        {
            var result = context.Computers .GroupBy(c => c.Brand).Select(g => new{Brand = g.Key,ComputerCount = g.Count(),AverageMemory = g.Average(c => (double)c.Memory),MaxMemory = g.Max(c => c.Memory)}).OrderBy(g => g.Brand).ToList();

            return new { message = "Sikeres lekérdezés", result };
        }


    }
}
