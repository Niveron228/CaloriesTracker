namespace CaloriesTracker.Models
{
    public class Meals
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public enum MealType
        {
            Breakfast,
            Lunch,
            Dinner,
            Snack
        }
    }
}
