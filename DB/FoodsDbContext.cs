using CaloriesTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace CaloriesTracker.DB
{
    public class FoodsDbContext : DbContext
    {
        public FoodsDbContext(DbContextOptions<FoodsDbContext> options) : base(options)
        {
        }

        public DbSet<Foods> Foods { get; set; }
    }
}
