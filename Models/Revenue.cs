namespace MauiCrud.Models
{
    public class Revenue
    {
        public int RevenueId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Client { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ArrivedDate { get; set; }
    }
}
