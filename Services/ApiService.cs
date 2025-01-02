using System.Net.Http.Json;
using MauiCrud.Models;

namespace MauiCrud.Services
{
    public class ApiService
    {
        private const string BaseUrl = "https://localhost:7236/api";

        public async Task<List<ExpenseCategory>> GetExpenseCategoriesAsync()
        {
            using var httpClient = new HttpClient();

            try
            {
                // Fetch data from the API
                var response = await httpClient.GetFromJsonAsync<List<ExpenseCategory>>($"{BaseUrl}/expensecategories");
                return response ?? new List<ExpenseCategory>();
            }
            catch (Exception ex)
            {
                // Handle errors (e.g., log or display a message)
                Console.WriteLine($"Error fetching data: {ex.Message}");
                return new List<ExpenseCategory>();
            }
        }
    }
}
