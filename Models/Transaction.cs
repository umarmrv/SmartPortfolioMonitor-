namespace SmartPortfolioMonitor.Models;

public class Transaction
{
    public int Id { get; set; }
    
    // Тикер актива: "BTC", "USD", "TJS"
    public required string Ticker { get; set; } 
    
    // Количество актива (например: 0.2 или 1500)
    public decimal Amount { get; set; } 
    
    // Цена покупки за ОДНУ единицу в долларах (например, 60000 для BTC или 1 для самого USD)
    public decimal PricePerUnitUSD { get; set; } 
    
    // Дата сделки
    public DateTime DateTime { get; set; } = DateTime.UtcNow;

    public int PortfolioId { get; set; }
    public Portfolio? Portfolio { get; set; }
}