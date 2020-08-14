using System.Security.Cryptography.X509Certificates;
using CurrencyExchange.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Services.DbConfiguration
{
    public class ExchangeRatesDbContext : DbContext
    {
        public ExchangeRatesDbContext(DbContextOptions<ExchangeRatesDbContext> options):base(options)
        {

        }

        public DbSet<ExchangeRate> ExchangeRates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExchangeRate>()
                .HasKey(rate => rate.Id);
        }
    }
}
