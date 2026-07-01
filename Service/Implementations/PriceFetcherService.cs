using System.Text.Json;
using SmartPortfolioMonitor.Services.Interfaces;

namespace SmartPortfolioMonitor.Services.Implementations;



public class PriceFetcherService : IPriceFetcherService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PriceFetcherService> _logger;

//setting up logger and httpclienFactory 

    public PriceFetcherService(HttpClient httpClient, ILogger<PriceFetcherService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }


    public async Task<decimal> GetCryptoPriceInUSDAsync(string ticker)
    {
        _logger.LogInformation("sendin http request to take a price of day of : {ticker}", ticker);

        try
        {
            var url = $"data/price?fsym={ticker.ToUpper()}&tsyms=USD";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode(); // if server return error 500 or 404 it will go the catch

            // Reading clean json 
            var jsonString = await response.Content.ReadAsStringAsync();

            //Parsing json deseirialization 

            using var doc = JsonDocument.Parse(jsonString);

            if (doc.RootElement.TryGetProperty("USD", out var priceElement))
            {
                decimal price = priceElement.GetDecimal();
                _logger.LogInformation("price for : {ticker} successfully got : {price}", ticker, price);
                return price;
            }
            
            _logger.LogWarning("API вернул странный ответ для {Ticker}: {Json}", ticker, jsonString);
            return 0m;
            
            
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при выполнении HTTP-запроса для тикера {Ticker}", ticker);
            throw; // Пробрасываем ошибку дальше, чтобы Математик о ней узнал
        }


    }
    }