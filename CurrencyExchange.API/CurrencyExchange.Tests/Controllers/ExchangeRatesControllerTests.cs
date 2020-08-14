using System.Collections.Generic;
using System.Linq;
using CurrencyExchange.API.Controllers;
using CurrencyExchange.Services.DbConfiguration;
using CurrencyExchange.Services.Dto;
using CurrencyExchange.Services.Enums;
using CurrencyExchange.Services.Interfaces;
using CurrencyExchange.Services.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Moq;
using Xunit;

namespace CurrencyExchange.Tests.Controllers
{
    public class ExchangeRatesControllerTests
    {
        private readonly Mock<IExchangeRatesService> _mockExchangeRatesService;
        private readonly ExchangeRatesController _controller;

        public ExchangeRatesControllerTests()
        {
            var options = new DbContextOptionsBuilder<ExchangeRatesDbContext>()
                .UseInMemoryDatabase(databaseName: "ExchangeRates")
                .Options;

            using (var context = new ExchangeRatesDbContext(options))
            {
                context.ExchangeRates.AddRange(ExchangeRateData);

                context.SaveChanges();
            }

            _mockExchangeRatesService = new Mock<IExchangeRatesService>();
            _controller =
                new ExchangeRatesController(new ExchangeRatesDbContext(options), _mockExchangeRatesService.Object);
        }

        [Fact]
        public async void GetCurrenciesAsync_ReturnsCurrencyList()
        {
            var result = _controller.GetCurrenciesAsync();

            Assert.IsAssignableFrom<ActionResult<IEnumerable<string>>>(result);
        }

        [Fact]
        public async void GetDatessAsync_ReturnsDateList()
        {
            var result = _controller.GetDatesAsync();

            Assert.IsAssignableFrom<ActionResult<IEnumerable<string>>>(result);
        }

        [Fact]
        public async void GetCurrencyConversionRateAsync_ReturnsCorrectResult()
        {
            var responseDto = new CurrencyConversionRateResponseDto()
            {
                ConversionRate = 1
            };

            _mockExchangeRatesService.Setup(x =>
                    x.GetCurrencyConversionRate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(responseDto);

            var result = _mockExchangeRatesService.Object.GetCurrencyConversionRate("EUR", "EUR", "2020-01-01");

            Assert.IsType<CurrencyConversionRateResponseDto>(result);
            Assert.Equal(responseDto.ConversionRate, result.ConversionRate);
        }

        [Fact]
        public async void GetExchangeRateHistoricDataAsync_ReturnsCorrectResult()
        {
            var responseDto = new ConversionRateHistoryDto()
            {
                FromCurrency = "EUR",
                ToCurrency = "EUR",
                DailyConversionRates = new List<DailyConversionRate>()
            };

            _mockExchangeRatesService.Setup(x =>
                    x.GetConversionRateHistoricDataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(responseDto);

            var result = _mockExchangeRatesService.Object.GetConversionRateHistoricDataAsync("EUR", "EUR");

            Assert.IsType<ConversionRateHistoryDto>(result);
            Assert.Equal(responseDto.FromCurrency, result.FromCurrency);
            Assert.Equal(responseDto.ToCurrency, result.ToCurrency);
            Assert.Empty(result.DailyConversionRates);
        }

        public static IEnumerable<ExchangeRate> ExchangeRateData =>
            new List<ExchangeRate>
            {
                new ExchangeRate() {Currency = "EUR", Date = "2020-01-01", Rate = 1},
                new ExchangeRate() {Currency = "BGD", Date = "2020-01-02", Rate = 1},
                new ExchangeRate() {Currency = "USD", Date = "2020-01-03", Rate = 1},
                new ExchangeRate() {Currency = "CAD", Date = "2020-01-04", Rate = 1}
            };
    }
}