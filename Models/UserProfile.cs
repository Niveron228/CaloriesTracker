namespace CaloriesTracker.Models
{
    public enum GoalType
    {
        LoseWeight = 1,  
        Maintain = 2,    
        GainWeight = 3    
    }
    public class UserProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Users User { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public int Age { get; set; }

        public GoalType Goal {  get; set; }
    }
}
