using System.Net.Http.Json;
using MauiCrud.Models;

namespace MauiCrud.Services
{
    public class ApiService1
    {
        private const string BaseUrl = "https://localhost:7236/api";
        private readonly HttpClient _httpClient = new();

        private async Task<T?> GetAsync<T>(string endpoint)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<T>($"{BaseUrl}/{endpoint}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data: {ex.Message}");
                return default;
            }
        }

        private async Task<bool> PostAsync<T>(string endpoint, T data)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/{endpoint}", data);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating data: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> PutAsync<T>(string endpoint, T data)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{endpoint}", data);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating data: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> DeleteAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{endpoint}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting data: {ex.Message}");
                return false;
            }
        }

        // Expense Category Methods
        public Task<List<ExpenseCategory>> GetExpenseCategoriesAsync()
            => GetAsync<List<ExpenseCategory>>("expensecategories") ?? Task.FromResult(new List<ExpenseCategory>());
        public Task<bool> CreateExpenseCategoryAsync(ExpenseCategory category)
            => PostAsync("expensecategory", category);
        public Task<bool> UpdateExpenseCategoryAsync(ExpenseCategory category)
            => PutAsync($"expensecategory/{category.ExpenseCategoryId}", category);
        public Task<bool> DeleteExpenseCategoryAsync(int id)
            => DeleteAsync($"expensecategory/{id}");

        // Expense Methods
        public Task<List<Expense>> GetExpensesAsync()
            => GetAsync<List<Expense>>("expenses") ?? Task.FromResult(new List<Expense>());
        public Task<bool> CreateExpenseAsync(Expense expense)
            => PostAsync("expenses", expense);
        public Task<bool> UpdateExpenseAsync(Expense expense)
            => PutAsync($"expenses/{expense.ExpenseId}", expense);
        public Task<bool> DeleteExpenseAsync(int id)
            => DeleteAsync($"expenses/{id}");

        // Revenue Methods
        public Task<List<Revenue>> GetRevenuesAsync()
            => GetAsync<List<Revenue>>("revenues") ?? Task.FromResult(new List<Revenue>());
        public Task<bool> CreateRevenueAsync(Revenue revenue)
            => PostAsync("revenues", revenue);
        public Task<bool> UpdateRevenueAsync(Revenue revenue)
            => PutAsync($"revenues/{revenue.RevenueId}", revenue);
        public Task<bool> DeleteRevenueAsync(int id)
            => DeleteAsync($"revenues/{id}");
    }
}
