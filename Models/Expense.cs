using MauiCrud.Models;
using SQLite;
public class Expense
{
    [PrimaryKey, AutoIncrement]
    public int ExpenseId { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime ExpenseDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public int ExpenseCategoryId { get; set; }

    [Ignore]
    public ExpenseCategory ExpenseCategory { get; set; }
}

