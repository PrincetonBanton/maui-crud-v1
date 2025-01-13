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

        // Check and update connectivity
        await ConnectivityService.Instance.CheckAndUpdateConnectivityAsync();

        // Fetch data based on default date values
        await FetchTotalsAsync();
    }

    private async void OnExpenseFrameTapped(object sender, EventArgs e)
        => await Navigation.PushAsync(new ExpensePage());

    private async void OnRevenueFrameTapped(object sender, EventArgs e)
        => await Navigation.PushAsync(new RevenuePage());

    private async void OnFetchDataClicked(object sender, EventArgs e)
        => await FetchTotalsAsync();

    private async Task FetchTotalsAsync()
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
        }
        catch (Exception ex)
        {
            // Display an error message if the operation fails
            await DisplayAlert("Error", $"Failed to fetch data: {ex.Message}", "OK");
        }
    }
}
