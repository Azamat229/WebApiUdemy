using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
             

        }

        public DbSet<Character> Character { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Weapon> Weapon { get; set; }

    }
}
 