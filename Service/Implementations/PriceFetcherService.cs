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
            var url = $"https://api.coinbase.com/v2/prices/{ticker.ToUpper()}-USD/spot";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode(); // if server return error 500 or 404 it will go the catch

            // Reading clean json 
            var jsonString = await response.Content.ReadAsStringAsync();

            //Parsing json deseirialization 

            using var doc = JsonDocument.Parse(jsonString);

                // 1.Сначала заходим в объект "data"
            if (doc.RootElement.TryGetProperty("data", out var dataElement))
            {
                // 2. Внутри "data" ищем свойство "amount"
                if (dataElement.TryGetProperty("amount", out var amountElement))
                {
                    // 3. Так как Coinbase возвращает цену в виде строки "60750.55", 
                    // сначала берем её как string, а потом парсим в decimal
                    string priceStr = amountElement.GetString() ?? "0";

                    if (decimal.TryParse(priceStr, System.Globalization.CultureInfo.InvariantCulture, out var price))
                    {
                        _logger.LogInformation("price for : {ticker} successfully got : {price}", ticker, price);
                        return price;
                    }
                }
            }

// Если что-то пошло не так, пишем лог и возвращаем 0
            _logger.LogWarning("API вернул странный ответ для {ticker}: {json}", ticker, jsonString);
            return 0m;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при выполнении HTTP-запроса для тикера {Ticker}", ticker);
            throw; // Пробрасываем ошибку дальше, чтобы Математик о ней узнал
        }


    }
    }