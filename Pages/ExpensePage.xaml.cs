using MauiCrud.Models;
using MauiCrud.Services;
using MauiCrud.Wrappers;

namespace MauiCrud.Pages
{
    public partial class ExpensePage : ContentPage
    {
        private Expense _currentExpense = new();
        private bool _isInternetAvailable;
        private readonly ApiService _apiService = new();
        private readonly DatabaseService _databaseService = new();

        public ExpensePage(Expense expense = null)
        {
            InitializeComponent();
            CheckConnectivity(); 
            _currentExpense = expense ?? new Expense();
            BindExpenseToForm();
        }

        private async void CheckConnectivity()
        {
            await ConnectivityService.Instance.CheckAndUpdateConnectivityAsync();
            _isInternetAvailable = ConnectivityService.Instance.IsInternetAvailable;
            internetStatusLabel.Text = _isInternetAvailable ? "Online" : "Offline";
            internetStatusLabel.TextColor = _isInternetAvailable ? Colors.Green : Colors.Red;

            if (_isInternetAvailable)
            {
                var localExpenses = await _databaseService.GetExpensesAsync();
                if (localExpenses.Any()) await MigrateLocalDataToApi(localExpenses);

                // Replace local ExpenseCategory table with API data
                var apiCategories = await _apiService.GetExpenseCategoryAsync();
                await _databaseService.ReplaceExpenseCategoryDataAsync(apiCategories);

                LoadOnlineData();
            } else {
                await DisplayAlert("Connectivity", "You are currently offline", "Ok");
                LoadOfflineData();
            }
        }

        private async Task MigrateLocalDataToApi(List<Expense> localExpenses)
        {
            foreach (var expense in localExpenses)
            {
                var apiExpense = new Expense
                {
                    ExpenseId = 0, // Ensure a new record is created on the server
                    Description = expense.Description,
                    Amount = expense.Amount,
                    ExpenseDate = expense.ExpenseDate,
                    ExpenseCategoryId = expense.ExpenseCategoryId
                    //ExpenseCategoryId = 1
                };

                try
                {
                    ApiResponse response = await _apiService.CreateExpenseAsync(apiExpense);
                    if (response.IsSuccess) await _databaseService.DeleteExpenseAsync(expense.ExpenseId);
                    else await DisplayAlert("Migration Error", $"Failed to migrate: {expense.Description}. ex: {response.Message}", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Failed to migrate expense: {expense.Description}. ex: {ex.Message} Check logs for details.", "OK");
                }
            }
            await DisplayAlert("Migration Complete", "All local expenses have been migrated to the API.", "OK");
        }
       
        private async void LoadOnlineData()
        {
            categoryPicker.ItemsSource = await _apiService.GetExpenseCategoryAsync();
            expenseListView.ItemsSource = await _apiService.GetExpensesAsync();
        }
        private async void LoadOfflineData()
        {
            categoryPicker.ItemsSource = await _databaseService.GetExpenseCategoryAsync();
            expenseListView.ItemsSource = await _databaseService.GetExpensesAsync();
        }
        private async Task<bool> ValidateExpenseInput()
        {
            if (string.IsNullOrWhiteSpace(descriptionEntry.Text))
            {
                await DisplayAlert("Validation Error", "Description is required.", "OK");
                return false;
            }

            if (!decimal.TryParse(amountEntry.Text, out var amount) || amount <= 0)
            {
                await DisplayAlert("Validation Error", "Please enter a valid amount greater than 0.", "OK");
                return false;
            }

            if (categoryPicker.SelectedItem is not ExpenseCategory)
            {
                await DisplayAlert("Validation Error", "Please select a category.", "OK");
                return false;
            }

            return true;
        }

        private async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            if (!await ValidateExpenseInput())
                return;

            _currentExpense.Description = descriptionEntry.Text;
            _currentExpense.Amount = decimal.Parse(amountEntry.Text);
            _currentExpense.ExpenseDate = expenseDatePicker.Date;
            _currentExpense.ExpenseCategoryId = ((ExpenseCategory)categoryPicker.SelectedItem).ExpenseCategoryId;

            if (_isInternetAvailable)
            {
                var result = _currentExpense.ExpenseId == 0
                    ? await _apiService.CreateExpenseAsync(_currentExpense)
                    : await _apiService.UpdateExpenseAsync(_currentExpense);
                LoadOnlineData();
            }
            else
            {
                var result = _currentExpense.ExpenseId == 0
                    ? await _databaseService.SaveExpenseAsync(_currentExpense)
                    : await _databaseService.UpdateExpenseAsync(_currentExpense);
                LoadOfflineData();
            }

            await DisplayAlert("Success", "Expense saved.", "OK");
            ClearForm();
        }


        private async void ExpenseListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is not Expense selectedExpense) return;

            string action = await DisplayActionSheet("Action", "Cancel", null, "Edit", "Delete");
            if (action == "Edit")
            {
                _currentExpense = selectedExpense;
                BindExpenseToForm();
            }
            else if (action == "Delete" && await DisplayAlert("Confirm", "Delete this expense?", "Yes", "No"))
            {
                if (_isInternetAvailable)
                {
                    var success = await _apiService.DeleteExpenseAsync(selectedExpense.ExpenseId);
                    LoadOnlineData();
                    await DisplayAlert(success ? "Success" : "Error", success ? "Expense deleted." : "Failed to delete expense.", "OK");
                }
                else
                {
                    var success = await _databaseService.DeleteExpenseAsync(selectedExpense.ExpenseId) > 0;
                    LoadOfflineData();
                    await DisplayAlert(success ? "Success" : "Error", success ? "Expense deleted offline." : "Failed to delete expense offline.", "OK");
                }
            }
            ((ListView)sender).SelectedItem = null;
        }
        private void BindExpenseToForm()
        {
            descriptionEntry.Text = _currentExpense.Description;
            amountEntry.Text = _currentExpense.Amount > 0 ? _currentExpense.Amount.ToString() : string.Empty;
            expenseDatePicker.Date = _currentExpense.ExpenseDate != default ? _currentExpense.ExpenseDate : DateTime.Now;
            categoryPicker.SelectedItem = (categoryPicker.ItemsSource as List<ExpenseCategory>)?
                .FirstOrDefault(c => c.ExpenseCategoryId == _currentExpense.ExpenseCategoryId);
        }
        private void ClearForm()
        {
            _currentExpense = new Expense();
            descriptionEntry.Text = string.Empty;
            amountEntry.Text = string.Empty;
            expenseDatePicker.Date = DateTime.Now;
            categoryPicker.SelectedItem = null;
        }

    }
}
