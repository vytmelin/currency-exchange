using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyExchange.Services.DbConfiguration;
using CurrencyExchange.Services.Dto;
using CurrencyExchange.Services.Interfaces;
using CurrencyExchange.Services.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.API.Controllers
{
    [Route("api/exchange-rates")]
    [ApiController]
    public class ExchangeRatesController : ControllerBase
    {
        private readonly ExchangeRatesDbContext _dbContext;
        private readonly IExchangeRatesService _exchangeRatesService;
        public ExchangeRatesController(ExchangeRatesDbContext dbContext, IExchangeRatesService exchangeRatesService)
        {
            _exchangeRatesService = exchangeRatesService;
            _dbContext = dbContext;
        }
        [HttpGet]
        [Route("historic-data")]
        public ActionResult<ConversionRateHistoryDto> GetExchangeRateHistoricDataAsync(string fromCurrency, string toCurrency)
        {
            var result = _exchangeRatesService.GetConversionRateHistoricDataAsync(fromCurrency, toCurrency);

            return Ok(result);
        }
        
        [HttpGet]
        [Route("currencies")]
        public ActionResult<IEnumerable<string>> GetCurrenciesAsync()
        {
            var result = _dbContext.ExchangeRates.Select(rate => rate.Currency).Distinct().OrderBy(curr => curr);

            return Ok(result);
        }

        [HttpGet]
        [Route("dates")]
        public ActionResult<IEnumerable<string>> GetDatesAsync()
        {
            var result = _dbContext.ExchangeRates.Select(rate => rate.Date).Distinct().OrderByDescending(dt => dt);

            return Ok(result);
        }

        [HttpGet]
        [Route("conversion-rate")]
        public ActionResult<CurrencyConversionRateResponseDto> GetCurrencyConversionRateAsync(string fromCurrency, string toCurrency, string date)
        {
            var result = _exchangeRatesService.GetCurrencyConversionRate(fromCurrency, toCurrency, date);

            return result;
        }
    }
}