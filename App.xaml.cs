using MauiCrud.Pages;
namespace MauiCrud
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new AppShell();
            //MainPage = new NavigationPage(new MainPage());
            MainPage = new NavigationPage (new ExpenseCategoryPage());
        }
    }
}
