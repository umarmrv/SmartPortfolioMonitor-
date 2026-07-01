using System.ComponentModel.DataAnnotations;

namespace SmartPortfolioMonitor.Dtos;

public class TransactionCreateDto
{
    [Required]
    [StringLength(10)]
    public string Ticker { get; set; } = string.Empty; // "BTC", "USD", "TJS"

    [Required]
    [Range(0.000001, double.MaxValue, ErrorMessage = "Количество должно быть больше нуля")]
    public decimal Amount { get; set; } // Сколько купил (например, 0.05 или 500)

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть больше нуля")]
    public decimal PricePerUnitUSD { get; set; } // По какой цене в долларах взял за 1 шт.

    [Required]
    public int PortfolioId { get; set; } // В какой именно портфель добавить сделку
}