namespace SmartPortfolioMonitor.Models;



public class User
{
    public int Id { get; set; }
    public  string Name { get; set; }
    public  string Email { get; set; }

    // Связь Один-ко-Многим: у одного юзера может быть много портфелей
    public List<Portfolio> Portfolios { get; set; } = new();
}