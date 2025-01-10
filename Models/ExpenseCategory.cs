using SQLite;

namespace MauiCrud.Models   
{
    public class ExpenseCategory
    {
        [PrimaryKey, AutoIncrement]
        public int ExpenseCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
