namespace MauiCrud.Pages;

public partial class DashBoard : ContentPage
{
    public DashBoard()
    {
        InitializeComponent();
    }
    private async void OnExpenseFrameTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ExpensePage());
    }

    private async void OnRevenueFrameTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RevenuePage());
    }

}