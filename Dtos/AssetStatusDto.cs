namespace SmartPortfolioMonitor.Dtos;

// Детализация по конкретному активу (например, по BTC)
public class AssetStatusDto
{
    public string Ticker { get; set; } = string.Empty; // "BTC", "USD"
    public decimal TotalAmount { get; set; }            // Сколько всего единиц на балансе
    public decimal CurrentPriceUSD { get; set; }        // Свежая цена из интернета за 1 единицу
    public decimal CurrentValueUSD { get; set; }        // Общая текущая стоимость (Amount * Price)
}