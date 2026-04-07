namespace CaloriesTracker.DTOs
{
    public class MealLogResultDto
    {
        public string FoodName { get; set; } = string.Empty;
        public double Calories { get; set; }
        public double Protein { get; set; }
        public double Fat { get; set; }
        public double Carbs { get; set; }
    }
}
