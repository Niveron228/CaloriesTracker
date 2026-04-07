namespace CaloriesTracker.DTOs
{
    public class DailyStatsDto
    {
        public string Date { get; set; } = string.Empty;
        public List<MealReportDto> Meals { get; set; } = new();
        public MacrosDto DayTotal { get; set; } = new();
    }

    public class MealReportDto
    {
        public string MealName { get; set; } = string.Empty;
        public double TotalCalories { get; set; }
        public double TotalProtein { get; set; }
        public double TotalFat { get; set; }
        public double TotalCarbs { get; set; }
        public int ItemsCount { get; set; }
        public List<MealItemDto> Items { get; set; } = new();
    }

    public class MealItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Grams { get; set; }
        public double Calories { get; set; }
        public double Protein { get; set; }
        public double Fat { get; set; }
        public double Carbs { get; set; }
    }

    public class MacrosDto
    {
        public double Calories { get; set; }
        public double Protein { get; set; }
        public double Fat { get; set; }
        public double Carbs { get; set; }
    }
}
