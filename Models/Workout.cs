namespace CaloriesTracker.Models
{
    public class Workout
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Users User { get; set; }

        public DateTime Date {  get; set; }

        public ICollection<WorkoutSet> WorkoutSets { get; set; } = new List<WorkoutSet>();
    }
}
