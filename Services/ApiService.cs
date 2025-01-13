using System.Net.Http.Json;
using MauiCrud.Models;

namespace MauiCrud.Services
{
    public class ApiService
    {
        private const string BaseUrl = "https://localhost:7236/api";
        private readonly HttpClient _httpClient = new();

        private async Task<T?> RequestAsync<T>(Func<Task<T?>> request, string errorMessage)
        {
            try { return await request(); }
            catch (Exception ex) { Console.WriteLine($"{errorMessage}: {ex.Message}"); return default; }
        }

        private Task<T?> GetAsync<T>(string endpoint)
            => RequestAsync(() => _httpClient.GetFromJsonAsync<T>($"{BaseUrl}/{endpoint}"), "Error fetching data");

        private Task<bool> PostAsync<T>(string endpoint, T data)
            => RequestAsync(async () =>
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/{endpoint}", data);
                return response.IsSuccessStatusCode;
            }, "Error creating data");

        private Task<bool> PutAsync<T>(string endpoint, T data)
            => RequestAsync(async () =>
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{endpoint}", data);
                return response.IsSuccessStatusCode;
            }, "Error updating data");

        private Task<bool> DeleteAsync(string endpoint)
            => RequestAsync(async () =>
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{endpoint}");
                return response.IsSuccessStatusCode;
            }, "Error deleting data");

        // Expense Category Methods
        public Task<List<ExpenseCategory>> GetExpenseCategoryAsync()
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

        //Date Filter
        public async Task<decimal?> GetTotalExpenseAsync(string startDate, string endDate)
        {
            var endpoint = $"totalexpense?startDate={startDate}&endDate={endDate}";
            return await GetAsync<decimal>(endpoint);
        }

        public async Task<decimal?> GetTotalIncomeAsync(string startDate, string endDate)
        {
            var endpoint = $"totalincome?startDate={startDate}&endDate={endDate}";
            return await GetAsync<decimal>(endpoint);
        }
    }
}
