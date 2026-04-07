namespace CaloriesTracker.DTOs
{
    public class UserProfileResponseDto
    {
        public string Email { get; set; } = string.Empty;
        public double Weight { get; set; }
        public double Height { get; set; }
        public int Age { get; set; }
        public string Goal { get; set; } = string.Empty;
    }
}
