using CaloriesTracker.DB;
using CaloriesTracker.DTOs;
using CaloriesTracker.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
namespace CaloriesTracker.Services.FoodsServices
{
    public class ApiService : IFoodApiService
    { 
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private string _accessToken = string.Empty;
        private readonly AppDbContext _context;
        public ApiService(HttpClient httpClient, IConfiguration config, AppDbContext context)
        {
            _httpClient = httpClient;
            _config = config;
            _context = context;
        }

        private async Task<string> GetAccessTokenAsync()
        {
            if (!string.IsNullOrEmpty(_accessToken)) return _accessToken;

            var clientId = _config["ExternalApi:ClientId"];
            var clientSecret = _config["ExternalApi:ClientSecret"];
            var tokenUrl = "https://oauth.fatsecret.com/connect/token";

            var credentials = $"{clientId}:{clientSecret}";
            var base64Credentials = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(credentials));

            var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl);

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64Credentials);

            var requestBody = new FormUrlEncodedContent(new[]
            {
        new KeyValuePair<string, string>("grant_type", "client_credentials"),
        new KeyValuePair<string, string>("scope", "basic")
    });
            request.Content = requestBody;
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"FatSecret Error: {response.StatusCode} - {errorContent}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<TokenResponse>(jsonResponse);

            _accessToken = tokenData.access_token; 
            return _accessToken;
        }

        public async Task<List<ExternalFoodDto>> SearchFoodAsync(string query)
        {
            var token = await GetAccessTokenAsync();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var encodedQuery = Uri.EscapeDataString(query);
            var searchUrl = $"https://platform.fatsecret.com/rest/foods/search/v1?search_expression={encodedQuery}&format=json";
            var response = await _httpClient.GetAsync(searchUrl);

         
            if (!response.IsSuccessStatusCode) return new List<ExternalFoodDto>();

           
            var jsonString = await response.Content.ReadAsStringAsync();

            
            var resultList = new List<ExternalFoodDto>();

           
            using JsonDocument doc = JsonDocument.Parse(jsonString);


            if (doc.RootElement.TryGetProperty("foods", out var foodsElement) &&
                foodsElement.TryGetProperty("food", out var foodArray))
            {
                if (foodArray.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in foodArray.EnumerateArray())
                    {
                        resultList.Add(new ExternalFoodDto
                        {
                            Name = item.GetProperty("food_name").GetString(),
                            Description = item.GetProperty("food_description").GetString()
                        });
                    }
                }
                else if (foodArray.ValueKind == JsonValueKind.Object)
                {
                    resultList.Add(new ExternalFoodDto
                    {
                        Name = foodArray.GetProperty("food_name").GetString(),
                        Description = foodArray.GetProperty("food_description").GetString()
                    });
                }
            }

            return resultList;
        }

        public double ParseNutritionValue(string description, string key)
        {
            var match = Regex.Match(description, $@"{key}:\s*([\d\.]+)", RegexOptions.IgnoreCase);
            if (match.Success && double.TryParse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
            {
                return value;
            }
            return 0;
        }

        public async Task<Foods?> ImportExternalFoodAsync(ExternalFoodDto dto)
        {
            var existingFood = await _context.Foods.FirstOrDefaultAsync(f => f.name.ToLower() == dto.Name.ToLower());

            if (existingFood != null)
            {
                return null;
            }

            double cal = ParseNutritionValue(dto.Description, "Calories");
            double p = ParseNutritionValue(dto.Description, "Protein");
            double f = ParseNutritionValue(dto.Description, "Fat");
            double c = ParseNutritionValue(dto.Description, "Carbs");

            var newLocalFood = new Foods
            {
                name = dto.Name,
                calories = Convert.ToInt32(cal),
                protein = Convert.ToInt32(p),
                fats = Convert.ToInt32(f),
                carbs = Convert.ToInt32(c)
            };

            await _context.Foods.AddAsync(newLocalFood);
            await _context.SaveChangesAsync();

            return newLocalFood;
        }

        public async Task SaveExternalListToDbAsync(List<ExternalFoodDto> externalList)
        {
            foreach (var externalItem in externalList)
            {
                await ImportExternalFoodAsync(externalItem);
            }
        }

    }
}
