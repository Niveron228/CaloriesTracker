namespace CaloriesTracker.DTOs
{
    public class AddWorkoutSetDto
    {
        public int? ExerciseId { get; set; } 
        public string? NewExerciseName { get; set; } 
        public string? MuscleGroup { get; set; }
        public double Weight { get; set; }
        public int Reps { get; set; }
        public DateTime Date { get; set; }
    }
}
