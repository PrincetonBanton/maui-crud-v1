using MauiCrud.Services;

namespace MauiCrud.Pages;

public partial class DashBoard : ContentPage
{
    private readonly ApiService _apiService = new();
    private bool _isSelectingStartDate = true;

    public DashBoard()
    {
        InitializeComponent();
        StartDateLabel.Text = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy"); // 1 month ago
        EndDateLabel.Text = DateTime.Now.ToString("MM/dd/yyyy"); // Today
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ConnectivityService.Instance.CheckAndUpdateConnectivityAsync();
        UpdateFontSize();
        await FetchTotalsAsync(showAlert: false);
    }

    private void OnDateSelected(object sender, DateChangedEventArgs e)
        => (sender == StartDatePicker ? StartDateLabel : EndDateLabel).Text = e.NewDate.ToString("MM/dd/yyyy");

    private async void OnFetchDataClicked(object sender, EventArgs e)
        => await FetchTotalsAsync(showAlert: true);

    private async Task FetchTotalsAsync(bool showAlert)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(StartDateLabel.Text) || string.IsNullOrWhiteSpace(EndDateLabel.Text) ||
                StartDateLabel.Text == "Select Start Date" || EndDateLabel.Text == "Select End Date")
            {
                await DisplayAlert("Error", "Please select a valid date range.", "OK");
                return;
            }

            var startDate = DateTime.Parse(StartDateLabel.Text).ToString("yyyy-MM-dd");
            var endDate = DateTime.Parse(EndDateLabel.Text).ToString("yyyy-MM-dd");

            var totalExpense = await _apiService.GetTotalExpenseAsync(startDate, endDate);
            var totalIncome = await _apiService.GetTotalIncomeAsync(startDate, endDate);

            ExpenseLabel.Text = totalExpense?.ToString("N2") ?? "0.00";
            RevenueLabel.Text = totalIncome?.ToString("N2") ?? "0.00";
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
        ExpenseLabel.Text = FormatCurrency(ExpenseLabel.Text);
        RevenueLabel.Text = FormatCurrency(RevenueLabel.Text);

        int fontSize = CalculateFontSize(ExpenseLabel.Text, RevenueLabel.Text);
        ExpenseLabel.FontSize = fontSize;
        RevenueLabel.FontSize = fontSize;
    }

    private string FormatCurrency(string value)
    {
        return decimal.TryParse(value, out decimal number) ? number.ToString("N2") : "0.00";
    }

    private int CalculateFontSize(string expenseText, string revenueText)
    {
        int maxLength = Math.Max(expenseText.Length, revenueText.Length);
        return Math.Max(50 - Math.Max(0, (maxLength - 6) * 5), 20);
    }

    private void OnDatePickerFocused(object sender, FocusEventArgs e)
    {
        if (sender is DatePicker datePicker)
            datePicker.TextColor = Colors.Red;
    }
    private void OnDatePickerUnfocused(object sender, FocusEventArgs e)
    {
        if (sender is DatePicker datePicker)
            datePicker.TextColor = Colors.Red;
    }

}
