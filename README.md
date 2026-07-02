КАК ДВИЖЕТСЯ ЗАПРОС В НАШЕМ ПРОЕКТЕ (Прямо по шагам):

1. Клиент (Swagger) шлет HTTP POST на /api/Portfolios/transaction с JSON внутри.
2. Program.cs принимает запрос, пропускает через Middleware (маршрутизацию).
3. Маршрутизация находит PortfoliosController (Контроллер).
4. Model Binding берет JSON и парсит его в C#-объект (TransactionCreateDto).
5. DI (Dependency Injection) собирает конструктор Контроллера, внедряя туда Scoped-сервис PortfolioService.
6. Контроллер вызывает метод сервиса. Service берет DbContext (тоже внедренный через DI).
7. Сначала сервис через LINQ проверяет, существует ли Портфель. EF Core делает SELECT EXISTS в Postgres.
8. Сервис создает новую сущность Transaction и делает Context.Transactions.Add(), а затем SaveChangesAsync(). EF Core генерирует команду INSERT INTO.
9. Математик (сервис) запрашивает цену актива. DI дает ему HttpClient. Сервис делает асинхронный GET на api.coinbase.com.
10. Мы получаем JSON от Coinbase, парсим его через JsonDocument (заходя в "data" -> "amount").
11. Математик считает: (Количество * Свежая цена) - Инвестировано.
12. Сервис маппит результат в PortfolioStatusDto и возвращает Контроллеру.
13. Контроллер возвращает Ok(result) -> ASP.NET превращает DTO обратно в JSON и отдает клиенту со статусом 200.
