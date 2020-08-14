using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using CurrencyExchange.Services.DbConfiguration;
using CurrencyExchange.Services.Dto;
using CurrencyExchange.Services.Enums;
using CurrencyExchange.Services.Interfaces;
using CurrencyExchange.Services.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CurrencyExchange.Services.Services
{
    public class ExchangeRatesService : IExchangeRatesService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ExchangeRatesService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task RefreshExchangeRatesAsync()
        {
            var exchangeRates = await GetHistoricExchangeRateDataAsync();

            var dbContext = BuildDbContext();

            exchangeRates.ToList().ForEach(rate => AddIfNotExists(rate, dbContext));

            await dbContext.SaveChangesAsync();

            await dbContext.DisposeAsync();
        }

        public ConversionRateHistoryDto GetConversionRateHistoricDataAsync(string fromCurrency, string toCurrency)
        {
            if (string.IsNullOrEmpty(fromCurrency) || string.IsNullOrEmpty(toCurrency))
            {
                return new ConversionRateHistoryDto()
                {
                    ErrorCode = ErrorCode.CurrencyNotProvided
                };
            }

            var dbContext = BuildDbContext();

            var rates = dbContext.ExchangeRates
                .Where(rate => rate.Currency == fromCurrency || rate.Currency == toCurrency);

            var distinctDates = rates.Select(rate => rate.Date).Distinct();

            var dto = new ConversionRateHistoryDto()
            {
                FromCurrency = fromCurrency,
                ToCurrency = toCurrency,
                DailyConversionRates = CalculateDailyConversionRates(distinctDates, rates, fromCurrency, toCurrency).OrderByDescending(rate => rate.Date)
            };

            return dto;
        }


        public async Task<IEnumerable<ExchangeRate>> GetHistoricExchangeRateDataAsync()
        {
            try
            {
                using var httpClient = _httpClientFactory.CreateClient();

                var response =
                    await httpClient.GetAsync(_configuration.GetValue<string>("ExchangeRatesHistoryServiceUrl"));

                XDocument document = XDocument.Parse(await response.Content.ReadAsStringAsync());

                if (document.Root != null)
                {
                    var exchangeRates = document.Root.Elements().Last().Elements()
                        .SelectMany(el => el.Elements(), (parent, child) => new ExchangeRate()
                        {
                            Currency = child.Attribute("currency")?.Value,
                            Rate = Decimal.Parse(child.Attribute("rate")?.Value ?? string.Empty),
                            Date = parent.Attribute("time")?.Value
                        });

                    var eurRates = exchangeRates.Select(rate => rate.Date).Distinct().Select(date => new ExchangeRate()
                    {
                        Date = date,
                        Currency = "EUR",
                        Rate = 1
                    });

                    return exchangeRates.Concat(eurRates);
                }

                return new List<ExchangeRate>();
            }
            catch (Exception)
            {
                // LOG ERROR
                throw;
            }
        }

        public CurrencyConversionRateResponseDto GetCurrencyConversionRate(string fromCurrency, string toCurrency,
            string date)
        {
            try
            {
                if (string.IsNullOrEmpty(fromCurrency) || string.IsNullOrEmpty(toCurrency))
                    return new CurrencyConversionRateResponseDto()
                    {
                        ErrorCode = ErrorCode.CurrencyNotProvided
                    };

                if (fromCurrency == toCurrency)
                    return new CurrencyConversionRateResponseDto()
                    {
                        ConversionRate = 1
                    };

                var dbContext = BuildDbContext();

                if (string.IsNullOrEmpty(date))
                    date = dbContext.ExchangeRates
                        .Select(rate => DateTime.ParseExact(rate.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture))
                        .OrderByDescending(dt => dt).FirstOrDefault().ToString("yyyy-MM-dd");


                var from = dbContext.ExchangeRates.FirstOrDefault(rate =>
                    rate.Currency == fromCurrency && rate.Date == date);
                var to = dbContext.ExchangeRates.FirstOrDefault(rate =>
                    rate.Currency == toCurrency && rate.Date == date);


                if (from == null || to == null)
                    return new CurrencyConversionRateResponseDto()
                    {
                        ErrorCode = ErrorCode.RateNotFound
                    };

                dbContext.DisposeAsync();

                return new CurrencyConversionRateResponseDto()
                {
                    ConversionRate = Decimal.Round(to.Rate / from.Rate, 5)
                };
            }
            catch (Exception)
            {
                // LOG ERROR
                throw;
            }
        }

        private void AddIfNotExists(ExchangeRate rate, ExchangeRatesDbContext dbContext)
        {
            if (dbContext.ExchangeRates.Any(storedExchangeRate =>
                storedExchangeRate.Date == rate.Date && storedExchangeRate.Currency == rate.Currency))
                return;

            dbContext.ExchangeRates.Add(rate);
        }

        private ExchangeRatesDbContext BuildDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ExchangeRatesDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "ExchangeRates");

            ExchangeRatesDbContext dbContext = new ExchangeRatesDbContext(optionsBuilder.Options);

            return dbContext;
        }

        private IEnumerable<DailyConversionRate> CalculateDailyConversionRates(IEnumerable<string> distinctDates,
            IEnumerable<ExchangeRate> rates, string from, string to)
        {
            var dailyConversionRates = new List<DailyConversionRate>();

            foreach (var date in distinctDates)
            {
                var toRate = rates.FirstOrDefault(rate => rate.Currency == to && rate.Date == date);
                var fromRate = rates.FirstOrDefault(rate => rate.Currency == from && rate.Date == date);

                if (toRate != null && fromRate != null)
                {
                    dailyConversionRates.Add(new DailyConversionRate()
                    {
                        Date = date,
                        Rate = Decimal.Round(toRate.Rate / fromRate.Rate, 5)
                    });
                }
                else
                {
                    dailyConversionRates.Add(new DailyConversionRate()
                    {
                        Date = date,
                        Rate = 0,
                        ErrorCode = ErrorCode.RateNotFound
                    });
                }
            }

            return dailyConversionRates;
        }
    }
}