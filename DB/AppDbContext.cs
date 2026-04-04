using CaloriesTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace CaloriesTracker.DB
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Foods> Foods { get; set; }

        public DbSet<Users> Users { get; set; }

        public DbSet<MealLog> mealLogs { get; set; }

        public DbSet<Workout> workouts { get; set; }
        public DbSet<Exercise> exercises { get; set; }
        public DbSet<WorkoutSet> workoutsSet { get; set; }


    }
}
