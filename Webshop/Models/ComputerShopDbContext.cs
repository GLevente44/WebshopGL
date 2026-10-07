using Microsoft.EntityFrameworkCore;

namespace Webshop.Models
{
    public class ComputerShopDbContext : DbContext
    {
        public ComputerShopDbContext(DbContextOptions options) : base(options)
        {
        }

        protected ComputerShopDbContext()
        {
        }

        public DbSet<Osystem> Osystems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=localhost;database=computer;user=root;password=");

        }
    }
}
