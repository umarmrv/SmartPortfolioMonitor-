namespace SmartPortfolioMonitor.Models;

public class Portfolio
{
    public int Id { get; set; }
    public string Name { get; set; } // Например, "Моя заначка" или "Крипта"
    
    public int UserId { get; set; }
    public User? User { get; set; }

    // Связь Один-ко-Многим: в одном портфеле много транзакций
    public List<Transaction> Transactions { get; set; } = new();
}