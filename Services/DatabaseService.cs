using SQLite;
using MauiCrud.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MauiCrud.Services
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseService()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MauiSqlite.db");
            _database = new SQLiteAsyncConnection(dbPath);

            // Drop and create tables
            //_database.ExecuteAsync("DROP TABLE IF EXISTS Expense").Wait(); // Drop existing table
            _database.CreateTableAsync<Expense>().Wait();
            _database.CreateTableAsync<ExpenseCategory>().Wait();
            _database.CreateTableAsync<Revenue>().Wait(); 
        }

        // Get Methods
        public Task<List<Expense>> GetExpensesAsync() => _database.Table<Expense>().ToListAsync();
        public Task<List<ExpenseCategory>> GetExpenseCategoryAsync() => _database.Table<ExpenseCategory>().ToListAsync();
        public Task<List<Revenue>> GetRevenuesAsync() => _database.Table<Revenue>().ToListAsync();

        // Save Methods
        public Task<int> SaveExpenseAsync(Expense expense) => _database.InsertAsync(expense);
        public Task<int> SaveExpenseCategoryAsync(ExpenseCategory category) => _database.InsertAsync(category);
        public Task<int> SaveRevenueAsync(Revenue revenue) => _database.InsertAsync(revenue);

        // Update Methods
        public Task<int> UpdateExpenseAsync(Expense expense) => _database.UpdateAsync(expense);
        public Task<int> UpdateExpenseCategoryAsync(ExpenseCategory category) => _database.UpdateAsync(category);
        public Task<int> UpdateRevenueAsync(Revenue revenue) => _database.UpdateAsync(revenue);

        // Delete Methods
        public Task<int> DeleteExpenseAsync(int id) => _database.DeleteAsync<Expense>(id);
        public Task<int> DeleteExpenseCategoryAsync(int id) => _database.DeleteAsync<ExpenseCategory>(id);
        public Task<int> DeleteRevenueAsync(int id) => _database.DeleteAsync<Revenue>(id);

        public async Task ReplaceExpenseCategoryDataAsync(IEnumerable<ExpenseCategory> categories)
        {
            try
            {
                await _database.DropTableAsync<ExpenseCategory>();
                await _database.CreateTableAsync<ExpenseCategory>();
                await _database.InsertAllAsync(categories);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error replacing ExpenseCategory data: {ex.Message}");
                throw;
            }
        }
    }
}
