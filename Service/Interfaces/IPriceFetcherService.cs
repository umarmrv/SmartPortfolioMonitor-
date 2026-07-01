namespace SmartPortfolioMonitor.Services.Interfaces;

public interface IPriceFetcherService
{
    // Метод принимает тикер (например, "BTC") и возвращает текущую цену в USD
    Task<decimal> GetCryptoPriceInUSDAsync(string ticker);
}

