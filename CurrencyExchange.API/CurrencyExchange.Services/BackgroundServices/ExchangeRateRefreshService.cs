using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CurrencyExchange.Services.DbConfiguration;
using CurrencyExchange.Services.Interfaces;
using CurrencyExchange.Services.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NCrontab;

namespace CurrencyExchange.Services.BackgroundServices
{
    public class ExchangeRateRefreshService : BackgroundService
    {
        private readonly CrontabSchedule _schedule;
        private DateTime _nextExecution;
        private readonly IExchangeRatesService _exchangeRatesService;

        public ExchangeRateRefreshService(IConfiguration configuration,
            IExchangeRatesService exchangeRatesService)
        {
            _schedule = CrontabSchedule.Parse(configuration.GetValue<string>("ExchangeRateRefreshCronSchedule"));
            _nextExecution = _schedule.GetNextOccurrence(DateTime.Now);
            _exchangeRatesService = exchangeRatesService;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            do
            {
                var now = DateTime.Now;
                if (now > _nextExecution)
                {
                    await _exchangeRatesService.RefreshExchangeRatesAsync();
                    _nextExecution = _schedule.GetNextOccurrence(DateTime.Now);
                }

                await Task.Delay(60000, cancellationToken); //Runs check every minute
            } while (!cancellationToken.IsCancellationRequested);
        }
    }
}