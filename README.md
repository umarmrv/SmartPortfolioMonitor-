# Smart Portfolio Monitor (Backend API)

**Smart Portfolio Monitor** — это высокопроизводительный RESTful Web API, разработанный на платформе .NET 8 (ASP.NET Core) для мониторинга, анализа и отслеживания эффективности инвестиционных портфелей в реальном времени.

Приложение позволяет агрегировать криптовалютные транзакции, автоматически вычислять среднюю стоимость позиций, запрашивать актуальные рыночные котировки через интеграцию с внешними провайдерами (Coinbase API) и выводить детальную финансовую аналитику (убытки, прибыль, общую стоимость активов).

---

## 🚀 Архитектурные особенности проекта

Проект построен с соблюдением принципов **SOLID**, **Clean Architecture (Слоистая архитектура)** и лучших практик разработки на C#:

* **Loose Coupling (Слабая связанность):** Взаимодействие между слоем представления (Controllers) и слоем бизнес-логики (Services) реализовано строго через абстракции (Интерфейсы). Это обеспечивает легкую заменяемость компонентов и расширяемость.
* **Dependency Injection (DI):** Инверсия управления (IoC) используется для гибкого управления жизненными циклами зависимостей (`Transient`, `Scoped`, `Singleton`).
* **In-Memory Caching (Singleton):** Для оптимизации сетевого трафика и защиты от лимитов внешних API (`Rate Limiting`), сервис котировок реализует потокобезопасный кэш в оперативной памяти с автоматическим обновлением данных.
* **Explicit Relationship Mapping:** Связи между сущностями (User, Portfolio, Transaction) настроены вручную с помощью **Fluent API** в Entity Framework Core.

---

## 🛠 Технологический стек

* **Runtime:** .NET 8 SDK / C# 12
* **Framework:** ASP.NET Core Web API
* **ORM:** Entity Framework Core (EF Core) 
* **Database:** PostgreSQL (через провайдер Npgsql)
* **External Integration:** HttpClient Factory & System.Text.Json (JsonDocument)
* **Logging:** Встроенный провайдер Microsoft.Extensions.Logging (Serilog-ready)
* **API Testing & Docs:** Swagger / OpenAPI

---

## 📁 Структура проекта

```text
SmartPortfolioMonitor/
│
├── Controllers/                 # Слой представления (HTTP-эндпоинты)
│   └── PortfoliosController.cs  # Обработка REST-запросов и привязка моделей (Model Binding)
│
├── Services/                    # Слой бизнес-логики
│   ├── Interfaces/              # Контракты/Интерфейсы сервисов (IPortfolioService, IPriceFetcherService)
│   └── Implementations/
│       ├── PortfolioService.cs     # "Математик" — расчет доходности, агрегация данных
│       └── PriceFetcherService.cs  # Интеграция с Coinbase API, парсинг JSON, Singleton-кэш
│
├── Models/                      # Сущности базы данных (Domain Models)
│   ├── User.cs
│   ├── Portfolio.cs
│   └── Transaction.cs
│
├── Dtos/                        # Объекты переноса данных (Data Transfer Objects)
│   ├── TransactionCreateDto.cs
│   ├── PortfolioUpdateDto.cs
│   └── PortfolioStatusDto.cs
│
├── Data/                        # Слой инфраструктуры данных
│   ├── ApplicationDbContext.cs  # Контекст БД и конфигурация Fluent API
│   └── Migrations/              # История миграций базы данных PostgreSQL
│
├── Properties/
│   └── launchSettings.json      # Настройки запуска (порты, профили)
│
├── appsettings.json             # Конфигурационный файл (Строки подключения, настройки логирования)
└── Program.cs                   # Точка входа, Middleware Pipeline и сборка DI-контейнера


⚙️ Локальный запуск и развертывание
Требования
Установленный .NET 8 SDK

Локально запущенная СУБД PostgreSQL

Настройка конфигурации
Перед запуском отредактируйте файл appsettings.json в корне проекта, указав актуальные параметры подключения к вашей базе данных PostgreSQL:

{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=smart_portfolio_db;Username=your_username;Password=your_password"
  }
}



Команды для запуска (в терминале)
Клонирование репозитория и переход в директорию:

Bash
cd Desktop/SmartPortfolioMonitor

Восстановление зависимостей (NuGet-пакетов):

Bash
dotnet restore

Применение миграций к базе данных PostgreSQL:

Bash
dotnet ef database update

Компиляция и запуск веб-сервера:

Bash
dotnet run

После успешного выполнения команды сервер начнет прослушивать порт. Откройте ваш браузер и перейдите по адресу:
👉 http://localhost:5211/swagger/index.html для интерактивного тестирования API через графический интерфейс Swagger
  
