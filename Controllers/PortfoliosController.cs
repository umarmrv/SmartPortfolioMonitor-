using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartPortfolioMonitor.Data;
using SmartPortfolioMonitor.Dtos;
using SmartPortfolioMonitor.Models;
using SmartPortfolioMonitor.Services.Interfaces;

namespace SmartPortfolioMonitor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PortfoliosController : ControllerBase
{
    private readonly IPortfolioService _portfolioService;
    private readonly ApplicationDbContext _context; // Нужен здесь только для прямой вставки транзакции
    private readonly ILogger<PortfoliosController> _logger;

    public PortfoliosController(
        IPortfolioService portfolioService, 
        ApplicationDbContext context, 
        ILogger<PortfoliosController> logger)
    {
        _portfolioService = portfolioService;
        _context = context;
        _logger = logger;
    }

    // 1. GET: api/portfolios/1
    // Возвращает полную аналитику: считает баланс и показывает, богатеет ли юзер
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPortfolioStatus(int id)
    {
        _logger.LogInformation("[API] Запрос на аналитику портфеля с ID: {PortfolioId}", id);

        try
        {
            var portfolioStatus = await _portfolioService.GetPortfolioStatusAsync(id);

            if (portfolioStatus == null)
            {
                _logger.LogWarning("[API] Портфель {PortfolioId} не найден в PostgreSQL", id);
                return NotFound(new { message = $"Портфель с ID {id} не найден." });
            }

            return Ok(portfolioStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[API] Ошибка при расчете портфеля {PortfolioId}", id);
            return StatusCode(500, new { message = "Внутренняя ошибка при расчете аналитики." });
        }
    }

    // 2. POST: api/portfolios/transaction
    // Позволяет докупить доллары, сомони или крипту. Данные изменятся динамически!
    [HttpPost("transaction")]
    public async Task<IActionResult> AddTransaction([FromBody] TransactionCreateDto dto)
    {
        _logger.LogInformation("[API] Запрос на добавление транзакции для {Ticker} в портфель {PortfolioId}", dto.Ticker, dto.PortfolioId);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // Проверяем, существует ли вообще такой портфель в базе
            var portfolioExists = await _context.Portfolios.AnyAsync(p => p.Id == dto.PortfolioId);
            if (!portfolioExists)
            {
                _logger.LogWarning("[API] Не удалось добавить транзакцию. Портфель {PortfolioId} не существует", dto.PortfolioId);
                return NotFound(new { message = $"Портфель с ID {dto.PortfolioId} не найден." });
            }

            // Маппим DTO в реальную модель базы данных
            var transaction = new Transaction
            {
                Ticker = dto.Ticker.ToUpper(),
                Amount = dto.Amount,
                PricePerUnitUSD = dto.PricePerUnitUSD,
                PortfolioId = dto.PortfolioId,
                DateTime = DateTime.UtcNow
            };

            // Сохраняем в PostgreSQL
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            _logger.LogInformation("[API] Транзакция ID {TransactionId} успешно сохранена в Postgres", transaction.Id);

            // Вместо пустого ответа возвращаем обновленный статус портфеля! 
            // Пользователь сразу увидит, как изменился его баланс и профит
            var updatedStatus = await _portfolioService.GetPortfolioStatusAsync(dto.PortfolioId);
            return Ok(updatedStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[API] Ошибка при сохранении транзакции");
            return StatusCode(500, new { message = "Не удалось сохранить транзакцию в базу данных." });
        }
    }
    
    // POST: api/portfolios
    [HttpPost]
    public async Task<IActionResult> CreatePortfolio([FromBody] string name)
    {
        // Так как юзер с ID 1 у нас уже точно сидирован миграцией в базе
        var portfolio = new Portfolio 
        { 
            Name = name, 
            UserId = 1 
        };

        _context.Portfolios.Add(portfolio);
        await _context.SaveChangesAsync();

        return Ok(portfolio);
    }
}