namespace CaloriesTracker.DTOs
{
    public class ExerciseMaxWeightDto
    {
        public int ExerciseId { get; set; }
        public string ExerciseName { get; set; } = string.Empty;
        public double MaxWeight { get; set; }
    }
}
