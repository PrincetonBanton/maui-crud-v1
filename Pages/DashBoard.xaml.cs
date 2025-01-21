using MauiCrud.Services;

namespace MauiCrud.Pages;

public partial class DashBoard : ContentPage
{
    private readonly ApiService _apiService = new();

    public DashBoard()
    {
        InitializeComponent();

        // Set default dates
        StartDatePicker.Date = DateTime.Now.AddMonths(-1); // 1 month ago
        EndDatePicker.Date = DateTime.Now; // Today
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ConnectivityService.Instance.CheckAndUpdateConnectivityAsync();
        UpdateFontSize();
        await FetchTotalsAsync(showAlert: false);
    }

    private async void OnFetchDataClicked(object sender, EventArgs e)
        => await FetchTotalsAsync(showAlert: true);

    private async Task FetchTotalsAsync(bool showAlert)
    {
        try
        {
            var startDate = StartDatePicker.Date.ToString("yyyy-MM-dd");
            var endDate = EndDatePicker.Date.ToString("yyyy-MM-dd");
            var totalExpense = await _apiService.GetTotalExpenseAsync(startDate, endDate);
            var totalIncome = await _apiService.GetTotalIncomeAsync(startDate, endDate);

 
            ExpenseLabel.Text = totalExpense?.ToString("F2") ?? "0.00";
            RevenueLabel.Text = totalIncome?.ToString("F2") ?? "0.00";
            UpdateFontSize();

            if (showAlert) await DisplayAlert("Success", "Totals successfully fetched.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to fetch data: {ex.Message}", "OK");
        }
    }

    private void UpdateFontSize()
    {
        // Get the current text values and format them with commas and 2 decimal places
        string expenseText = ExpenseLabel.Text ?? "0.00";
        string revenueText = RevenueLabel.Text ?? "0.00";

        // Format the values with commas and 2 decimal places
        string formattedExpenseText = decimal.TryParse(expenseText, out decimal expenseValue)
            ? expenseValue.ToString("N2")
            : expenseText;

        string formattedRevenueText = decimal.TryParse(revenueText, out decimal revenueValue)
            ? revenueValue.ToString("N2")
            : revenueText;

        // Update the labels with formatted values
        ExpenseLabel.Text = formattedExpenseText;
        RevenueLabel.Text = formattedRevenueText;

        int fontSize = CalculateFontSize(formattedExpenseText, formattedRevenueText);

        ExpenseLabel.FontSize = fontSize;
        RevenueLabel.FontSize = fontSize;
    }
    private int CalculateFontSize(string expenseText, string revenueText)
    {
        int maxLength = Math.Max(expenseText.Length, revenueText.Length);
        int fontSize = 50 - Math.Max(0, (maxLength - 6) * 5);
        return Math.Max(fontSize, 20);
    }
}
