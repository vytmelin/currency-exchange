using CurrencyExchange.Services.BackgroundServices;
using CurrencyExchange.Services.DbConfiguration;
using CurrencyExchange.Services.Interfaces;
using CurrencyExchange.Services.Models;
using CurrencyExchange.Services.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyExchange.API.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddTransient<IExchangeRatesService, ExchangeRatesService>();

            services.AddDbContext<ExchangeRatesDbContext>(options => options.UseInMemoryDatabase(databaseName: "ExchangeRates"));

            services.AddHostedService<ExchangeRateRefreshService>();

            return services;
        }
    }
}
