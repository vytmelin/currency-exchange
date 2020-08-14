using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CurrencyExchange.Services.Dto;
using CurrencyExchange.Services.Models;

namespace CurrencyExchange.Services.Interfaces
{
    public interface IExchangeRatesService
    {
        Task<IEnumerable<ExchangeRate>> GetHistoricExchangeRateDataAsync();
        Task RefreshExchangeRatesAsync();
        ConversionRateHistoryDto GetConversionRateHistoricDataAsync(string fromCurrency, string toCurrency);
        CurrencyConversionRateResponseDto GetCurrencyConversionRate(string fromCurrency, string toCurrency, string date);
    }
}
