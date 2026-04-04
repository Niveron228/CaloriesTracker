namespace CaloriesTracker.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MuscleGroup { get; set; }
        public int? UserId { get; set; }
        public Users User { get; set; }
    }
}
