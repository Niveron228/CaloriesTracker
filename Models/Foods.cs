namespace CaloriesTracker.Models
{
    public class Foods
    {
        public int Id {  get; set; }
        public string name { get; set; } = string.Empty;
        public int calories {  get; set; }
        public int protein { get; set; }
        public int fats { get; set; }
        public int carbs { get; set; }

        public Foods() { }
    }
}
