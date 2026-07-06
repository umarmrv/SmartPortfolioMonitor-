using Microsoft.EntityFrameworkCore;
using SmartPortfolioMonitor.Data;
using SmartPortfolioMonitor.Dtos;
using SmartPortfolioMonitor.Services.Interfaces;

namespace SmartPortfolioMonitor.Services.Implementations;

public class PortfolioService : IPortfolioService
{
    private readonly ApplicationDbContext _context;
    private readonly IPriceFetcherService _priceFetcher;
    private readonly ILogger<PortfolioService> _logger;

    // Внедряем контекст Postgres, логгер и наш сервис-разведчик HttpClient
    public PortfolioService(ApplicationDbContext context, IPriceFetcherService priceFetcher, ILogger<PortfolioService> logger)
    {
        _context = context;
        _priceFetcher = priceFetcher;
        _logger = logger;
    }

    
    
    //For GETting Profile Status inside we used other service  Http getter which will get info from APi
    //==================================================================================================================
    public async Task<PortfolioStatusDto?> GetPortfolioStatusAsync(int portfolioId)
    {
        _logger.LogInformation("Математик начинает расчет для портфеля {PortfolioId}", portfolioId);

        // 1. Достаем портфель из PostgreSQL вместе с его транзакциями
        var portfolio = await _context.Portfolios
            .Include(p => p.Transactions)
            .FirstOrDefaultAsync(p => p.Id == portfolioId);

        if (portfolio == null) return null;

        // 2. Группируем транзакции по тикерам (собираем все BTC вместе, все USD вместе)
        var groupedTransactions = portfolio.Transactions
            .GroupBy(t => t.Ticker.ToUpper())
            .ToList();

        var statusDto = new PortfolioStatusDto
        {
            PortfolioId = portfolio.Id,
            PortfolioName = portfolio.Name
        };

        // 3. Считаем математику для каждой группы активов
        foreach (var group in groupedTransactions)
        {
            string ticker = group.Key;
            
            // Считаем общее количество этого актива на балансе
            decimal totalAmount = group.Sum(t => t.Amount);
            if (totalAmount <= 0) continue; // Если всё продано, пропускаем

            // Сколько долларов было вложено изначально
            decimal totalSpentOnAsset = group.Sum(t => t.Amount * t.PricePerUnitUSD);

            // По умолчанию цена 1$ (для самого доллара)
            decimal currentPriceUSD = 1m;

            // Если это крипта (BTC), отправляем разведчика в интернет за свежей ценой
            if (ticker != "USD")
            {
                try
                {
                    currentPriceUSD = await _priceFetcher.GetCryptoPriceInUSDAsync(ticker);
                }
                catch
                {
                    _logger.LogWarning("Не удалось получить свежую цену для {Ticker}. Используем $0.", ticker);
                    currentPriceUSD = 0m;
                }
            }

            // Сколько этот объем стоит СЕЙЧАС по рыночной цене
            decimal currentValueUSD = totalAmount * currentPriceUSD;

            // Плюсуем в общую копилку портфеля
            statusDto.TotalInvestedUSD += totalSpentOnAsset;
            statusDto.TotalValueUSD += currentValueUSD;

            // Добавляем актив в итоговый список
            statusDto.Assets.Add(new AssetStatusDto
            {
                Ticker = ticker,
                TotalAmount = totalAmount,
                CurrentPriceUSD = currentPriceUSD,
                CurrentValueUSD = currentValueUSD
            });
        }

        // 4. Финальный расчет чистой прибыли/убытка
        statusDto.TotalProfitLossUSD = statusDto.TotalValueUSD - statusDto.TotalInvestedUSD;
        statusDto.StatusMessage = statusDto.TotalProfitLossUSD >= 0 
            ? "Ты богатеешь! 📈" 
            : "Рынок идет против тебя, ты в минусе! 📉";

        _logger.LogInformation("Расчет портфеля {PortfolioId} успешно завершен.", portfolioId);
        
        return statusDto;
    }
    
    
    
    // Service method wich will update Profile table name space 
    //==================================================================================================================
    public async Task<bool> UpdatePortfolioAsync(int id, PortfolioUpdateDto dto)
    {
        // 1. Ищем портфель в базе по ID
        var portfolio = await _context.Portfolios.FindAsync(id);
    
        // Если портфель не найден, возвращаем false, чтобы контроллер выдал 404
        if (portfolio == null)
        {
            _logger.LogWarning("Попытка обновить несуществующий портфель с ID: {Id}", id);
            return false;
        }

        // 2. Обновляем свойства из DTO
        portfolio.Name = dto.Name;

        // 3. Сохраняем изменения в Postgres
        await _context.SaveChangesAsync();
    
        _logger.LogInformation("Портфель с ID: {Id} успешно обновлен. Новое имя: {Name}", id, dto.Name);
        return true;
    }
    
    
    
    public async Task<bool> DeletePortfolioAsync(int id)
    {
        // 1. Ищем портфель в базе данных по ID
        var portfolio = await _context.Portfolios.FindAsync(id);
    
        // Если такого портфеля нет, возвращаем false
        if (portfolio == null)
        {
            _logger.LogWarning("Попытка удалить несуществующий портфель с ID: {Id}", id);
            return false;
        }

        // 2. Командуем EF Core удалить объект
        _context.Portfolios.Remove(portfolio);

        // 3. Фиксируем удаление в PostgreSQL (сгенерируется SQL-команда DELETE FROM)
        await _context.SaveChangesAsync();

        _logger.LogInformation("Портфель с ID: {Id} был успешно удален из базы данных", id);
        return true;
    }
    
    
    
    
    
    
    
}