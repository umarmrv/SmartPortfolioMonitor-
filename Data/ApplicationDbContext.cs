using Microsoft.EntityFrameworkCore;
using SmartPortfolioMonitor.Models;

namespace SmartPortfolioMonitor.Data;



public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}
    
    
    public DbSet<Portfolio> Portfolios { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<User> Users {get; set;}



    protected override  void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<Portfolio>()
            .HasOne(p => p.User)
            .WithMany(u => u.Portfolios)
            .HasForeignKey(p => p.UserId);
        
        
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Portfolio)
            .WithMany(p => p.Transactions)
            .HasForeignKey(t => t.PortfolioId);
        
        
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Name = "Shohjahon", Email = "shoh@example.com" }
        );
        
        
        modelBuilder.Entity<Portfolio>().HasData(
            new Portfolio { Id = 1, Name = "Основной Портфель", UserId = 1 }
        );
        
        modelBuilder.Entity<Transaction>().HasData(
            // Купил 0.1 BTC по цене 60 000$
            new Transaction { Id = 1, Ticker = "BTC", Amount = 0.1m, PricePerUnitUSD = 60000m, PortfolioId = 1 },
            // Отложил 1000 наличных долларов (цена за 1 USD = 1$)
            new Transaction { Id = 2, Ticker = "USD", Amount = 1000m, PricePerUnitUSD = 1m, PortfolioId = 1 }
        );
    }
    
    
    
}