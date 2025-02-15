using MauiCrud.Models;
using MauiCrud.Wrappers;
using System.Net.Http.Json;

namespace MauiCrud.Services
{
    public class ApiService
    {
        private const string BaseUrl = "https://localhost:7236/api";
        //private const string BaseUrl = "http://192.168.254.116:8080/api";
        private readonly HttpClient _httpClient = new();

        private async Task<T?> RequestAsync<T>(Func<Task<T?>> request, string errorMessage)
        {
            try { return await request(); }
            catch (Exception ex) { Console.WriteLine($"{errorMessage}: {ex.Message}"); return default; }
        }
        
        private async Task<T> GetAsync<T>(string endpoint) where T : class, new()
        {
            var result = await RequestAsync(() => _httpClient.GetFromJsonAsync<T>($"{BaseUrl}/{endpoint}"), "Error fetching data");
            return result ?? new T(); // Ensure result is never null
        }

        private Task<ApiResponse> PostAsync<T>(string endpoint, T data)
        {
            return RequestAsync(async () =>
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/{endpoint}", data);
                var result = new ApiResponse
                {
                    IsSuccess = response.IsSuccessStatusCode,
                    Message = response.IsSuccessStatusCode ? "Request successful" : $"Error: {response.ReasonPhrase}"
                };

                if (response.IsSuccessStatusCode)
                {
                    result.Result = await response.Content.ReadAsStringAsync(); // Deserialize if needed
                }

                return result;
            }, "Error creating data")!; // Ensure non-nullable return type
        }
        
        private async Task<ApiResponse> PutAsync<T>(string endpoint, T data)
        {
            var result = await RequestAsync(async () =>
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{endpoint}", data);
                var apiResponse = new ApiResponse
                {
                    IsSuccess = response.IsSuccessStatusCode,
                    Message = response.IsSuccessStatusCode ? "Request successful" : $"Error: {response.ReasonPhrase}"
                };

                if (response.IsSuccessStatusCode)
                {
                    apiResponse.Result = await response.Content.ReadAsStringAsync(); // Deserialize if needed
                }

                return apiResponse;
            }, "Error updating data");

            return result ?? new ApiResponse { IsSuccess = false, Message = "Unknown error" };
        }

        private Task<bool> DeleteAsync(string endpoint)
            => RequestAsync(async () =>
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{endpoint}");
                return response.IsSuccessStatusCode;
            }, "Error deleting data");
        
        #region Expense Category Methods
        public Task<List<ExpenseCategory>> GetExpenseCategoryAsync()
            => GetAsync<List<ExpenseCategory>>("expensecategories") ?? Task.FromResult(new List<ExpenseCategory>());
        public Task<ApiResponse> CreateExpenseCategoryAsync(ExpenseCategory category)
            => PostAsync("expensecategory", category);
        public Task<ApiResponse> UpdateExpenseCategoryAsync(ExpenseCategory category)
            => PutAsync($"expensecategory/{category.ExpenseCategoryId}", category);
        public Task<bool> DeleteExpenseCategoryAsync(int id)
            => DeleteAsync($"expensecategory/{id}");
        #endregion

        #region Expense Methods
        public Task<List<Expense>> GetExpensesAsync() => GetAsync<List<Expense>>("expenses");
        public Task<ApiResponse> CreateExpenseAsync(Expense expense)
            => PostAsync("expenses", expense);
        public Task<ApiResponse> UpdateExpenseAsync(Expense expense)
            => PutAsync($"expenses/{expense.ExpenseId}", expense);
        public Task<bool> DeleteExpenseAsync(int id)
            => DeleteAsync($"expenses/{id}"); 
        #endregion

        #region Revenue Methods
        public Task<List<Revenue>> GetRevenuesAsync() => GetAsync<List<Revenue>>("revenues");

        public Task<ApiResponse> CreateRevenueAsync(Revenue revenue)
            => PostAsync("revenues", revenue);
        public Task<ApiResponse> UpdateRevenueAsync(Revenue revenue)
            => PutAsync($"revenues/{revenue.RevenueId}", revenue);
        public Task<bool> DeleteRevenueAsync(int id)
            => DeleteAsync($"revenues/{id}");
        #endregion

        #region Date Filter
        public async Task<decimal?> GetTotalExpenseAsync(string startDate, string endDate)
        {
            var endpoint = $"totalexpense?startDate={startDate}&endDate={endDate}";
            var result = await RequestAsync(() => _httpClient.GetFromJsonAsync<decimal?>($"{BaseUrl}/{endpoint}"), "Error fetching data");
            return result;
        }

        public async Task<decimal?> GetTotalIncomeAsync(string startDate, string endDate)
        {
            var endpoint = $"totalincome?startDate={startDate}&endDate={endDate}";
            var result = await RequestAsync(() => _httpClient.GetFromJsonAsync<decimal?>($"{BaseUrl}/{endpoint}"), "Error fetching data");
            return result;
        } 
        #endregion
    }
}
