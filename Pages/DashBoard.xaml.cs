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
        await FetchTotalsAsync(showAlert: false);
    }

    private async void OnFetchDataClicked(object sender, EventArgs e)
        => await FetchTotalsAsync(showAlert: true);

    private async Task FetchTotalsAsync(bool showAlert)
    {
        try
        {
            // Retrieve the selected dates
            var startDate = StartDatePicker.Date.ToString("yyyy-MM-dd");
            var endDate = EndDatePicker.Date.ToString("yyyy-MM-dd");

            // Fetch data from the API
            var totalExpense = await _apiService.GetTotalExpenseAsync(startDate, endDate);
            var totalIncome = await _apiService.GetTotalIncomeAsync(startDate, endDate);

            // Update UI labels
            ExpenseLabel.Text = totalExpense?.ToString("F2") ?? "0.00";
            RevenueLabel.Text = totalIncome?.ToString("F2") ?? "0.00";

            if (showAlert)
            {
                await DisplayAlert("Success", "Totals successfully fetched.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to fetch data: {ex.Message}", "OK");
        }
    }

}
