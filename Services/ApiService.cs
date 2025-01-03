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

        // Create a new expense category
        public async Task<bool> CreateExpenseCategoryAsync(ExpenseCategory category)
        {
            using var httpClient = new HttpClient();

            try
            {
                var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/expensecategory", category);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating data: {ex.Message}");
                return false;
            }
        }

        // Update an expense category
        public async Task<bool> UpdateExpenseCategoryAsync(ExpenseCategory category)
        {
            using var httpClient = new HttpClient();

            try
            {
                var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/expensecategory/{category.ExpenseCategoryId}", category);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating data: {ex.Message}");
                return false;
            }
        }

        // Delete an expense category
        public async Task<bool> DeleteExpenseCategoryAsync(int id)
        {
            using var httpClient = new HttpClient();

            try
            {
                var response = await httpClient.DeleteAsync($"{BaseUrl}/expensecategory/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting data: {ex.Message}");
                return false;
            }
        }

        //EXPENSE
        public async Task<List<Expense>> GetExpensesAsync()
        {
            using var httpClient = new HttpClient();
            try
            {
                // Fetch data from the API
                var response = await httpClient.GetFromJsonAsync<List<Expense>>($"{BaseUrl}/expenses");
                return response ?? new List<Expense>();
            }
            catch (Exception ex)
            {
                // Handle errors (e.g., log or display a message)
                Console.WriteLine($"Error fetching expenses: {ex.Message}");
                return new List<Expense>();
            }
        }

        // Create a new expense
        public async Task<bool> CreateExpenseAsync(Expense expense)
        {
            using var httpClient = new HttpClient();

            try
            {
                var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/expenses", expense);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating expense: {ex.Message}");
                return false;
            }
        }

        // Update an existing expense
        public async Task<bool> UpdateExpenseAsync(Expense expense)
        {
            using var httpClient = new HttpClient();

            try
            {
                var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/expenses/{expense.ExpenseId}", expense);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating expense: {ex.Message}");
                return false;
            }
        }

        // Delete an expense
        public async Task<bool> DeleteExpenseAsync(int id)
        {
            using var httpClient = new HttpClient();

            try
            {
                var response = await httpClient.DeleteAsync($"{BaseUrl}/expenses/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting expense: {ex.Message}");
                return false;
            }
        }


    }

}
