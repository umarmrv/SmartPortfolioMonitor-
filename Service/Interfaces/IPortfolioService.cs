using SmartPortfolioMonitor.Dtos;

namespace SmartPortfolioMonitor.Services.Interfaces;

public interface IPortfolioService
{
    // Этот метод будет возвращать готовый DTO-отчет с аналитикой портфеля
    Task<PortfolioStatusDto?> GetPortfolioStatusAsync(int portfolioId);
}