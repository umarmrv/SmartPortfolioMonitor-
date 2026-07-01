using Microsoft.EntityFrameworkCore;
using Serilog;
using SmartPortfolioMonitor.Data;
using SmartPortfolioMonitor.Services.Implementations;
using SmartPortfolioMonitor.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. НАСТРОЙКА ЛОГИРОВАНИЯ (SERILOG)
// ==========================================
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() // Пишем логи в консоль Rider красивым цветом
    .WriteTo.File("Logs/portfolio_monitor_.txt", rollingInterval: RollingInterval.Day) // Каждый день создаем новый файл лога
    .CreateLogger();

// Говорим .NET использовать Serilog вместо стандартного встроенного логгера
builder.Host.UseSerilog();  

// ==========================================
// 2. РЕГИСТРАЦИЯ СЕРВИСОВ В DI-КОНТЕЙНЕРЕ
// ==========================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Наш PostgreSQL контекст (Scoped по умолчанию)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

// РЕГИСТРИРУЕМ НАШИ КАСТОМНЫЕ СЕРВИСЫ:

// 1. Сервис-разведчик (Типизированный HttpClient с поддержкой HttpClientFactory).
// Фабрика сама управляет пулом соединений, защищая сервер от исчерпания сокетов.
builder.Services.AddHttpClient<IPriceFetcherService, PriceFetcherService>(client =>
{
    // Задаем базовый адрес для всех будущих запросов этого сервиса
    client.BaseAddress = new Uri("https://min-api.cryptocompare.com/");
    
    // Предохранитель: если внешнее API зависнет, мы разорвем соединение через 10 секунд
    client.Timeout = TimeSpan.FromSeconds(10);
    
    // Добавляем обязательный заголовок, сообщая внешнему серверу, что ждем ответ в JSON
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// 2. Сервис-математик (Бизнес-логика). Регистрируем как Scoped.
// Почему строго Scoped? Потому что внутри него используется ApplicationDbContext!
builder.Services.AddScoped<IPortfolioService, PortfolioService>();


var app = builder.Build();

// ==========================================
// 3. КОНВЕЙЕР MIDDLEWARE
// ==========================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Добавляем мидлварь от Serilog для автоматического логирования входящих HTTP-запросов
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

try
{
    Log.Information("Запуск сервера Smart Portfolio Monitor...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Сервер упал при запуске!");
}
finally
{
    Log.CloseAndFlush();
}