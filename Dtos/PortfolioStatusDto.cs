namespace SmartPortfolioMonitor.Dtos;

// Главный плоский объект отчета по всему портфелю
public class PortfolioStatusDto
{
    public int PortfolioId { get; set; }
    public string PortfolioName { get; set; } = string.Empty;
    public decimal TotalValueUSD { get; set; }      // Сколько все активы стоят СЕЙЧАС в $
    public decimal TotalInvestedUSD { get; set; }   // Сколько денег было вложено изначально в $
    public decimal TotalProfitLossUSD { get; set; } // Чистый профит (плюс или минус) в $
    public string StatusMessage { get; set; } = string.Empty; // "Ты богатеешь!" или "В минусе"

    // Список сгруппированных активов пользователя
    public List<AssetStatusDto> Assets { get; set; } = new();
}