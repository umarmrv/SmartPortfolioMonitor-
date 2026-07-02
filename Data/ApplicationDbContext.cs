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
        //
        //
        // modelBuilder.Entity<Portfolio>().HasData(
        //     new Transaction 
        //     { 
        //         Id = 1, 
        //         Ticker = "BTC", 
        //         Amount = 0.1m, 
        //         PricePerUnitUSD = 60000m, 
        //         PortfolioId = 1,
        //         DateTime = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc) // Фиксированная дата!
        //     },
        //     new Transaction 
        //     { 
        //         Id = 2, 
        //         Ticker = "USD", 
        //         Amount = 1000m, 
        //         PricePerUnitUSD = 1m, 
        //         PortfolioId = 1,
        //         DateTime = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc) // Фиксированная дата!
        //     }
        // );
    }
    
    
    
}