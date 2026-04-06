namespace CaloriesTracker.DTOs
{
    public class WorkoutHistoryResponse
    {
        public int SetId { get; set; } 
        public int ExerciseId {  get; set; }
        public string ExerciseName { get; set; } 
        public string MuscleGroup { get; set; }
        public double Weight { get; set; }
        public int Reps { get; set; }
        public DateTime Date { get; set; }
    }
}
