using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [HttpGet("getOsystemByComputer")]
        public object GetOsystemByComputer() 
        {
            var result = context.Osystems.Include(os => os.Computers).ToList();
            return new { message = "Sikeres lekérdezés", result = "" };
        }
        // 2. Feladat
        [HttpGet("getAllComputerbyType")]
        public object GetAllComputerbyType()
        {
            var result = context.Computers.Include(cmp => cmp.Type).ToList();
            return new { message = "Sikeres lekérdezés", result = "" };
        }

        // 3. Feladat
        





    }
}
