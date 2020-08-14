using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CurrencyExchange.Services.DbConfiguration;
using CurrencyExchange.Services.Dto;
using CurrencyExchange.Services.Enums;
using CurrencyExchange.Services.Interfaces;
using CurrencyExchange.Services.Models;
using CurrencyExchange.Services.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using Xunit;

namespace CurrencyExchange.Tests.Services
{
    public class ExchangeRatesServiceTests : IAsyncLifetime
    {
        private readonly IExchangeRatesService _service;

        public async Task InitializeAsync()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<ExchangeRatesDbContext>();
            builder.UseInMemoryDatabase(databaseName: "ExchangeRates")
                .UseInternalServiceProvider(serviceProvider);

            await _service.RefreshExchangeRatesAsync();
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public ExchangeRatesServiceTests()
        {
            var configurationSectionMock = new Mock<IConfigurationSection>();
            configurationSectionMock.Setup(x => x.Value)
                .Returns("https://www.ecb.europa.eu/stats/eurofxref/eurofxref-hist-90d.xml");
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x.GetSection(It.IsAny<string>()))
                .Returns(configurationSectionMock.Object);
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() => new HttpClient());

            _service = new ExchangeRatesService(configurationMock.Object, mockHttpClientFactory.Object);
        }

        [Fact]
        public async void GetHistoricExchangeRateDataAsync_ReturnsExchangeRates()
        {
            var result = await _service.GetHistoricExchangeRateDataAsync();

            Assert.IsAssignableFrom<IEnumerable<ExchangeRate>>(result);
        }

        [Theory]
        [MemberData(nameof(CurrencyConversionRateTestData))]
        public async void GetCurrencyConversionRate_ReturnsCorrectResult(string from, string to, string date,
            decimal? expectedRate, ErrorCode expectedErrorCode)
        {
            var result = _service.GetCurrencyConversionRate(from, to, date);

            Assert.IsType<CurrencyConversionRateResponseDto>(result);
            Assert.Equal(expectedRate, result.ConversionRate);
            Assert.Equal(expectedErrorCode, result.ErrorCode);
        }

        [Theory]
        [MemberData(nameof(CurrencyConversionHistoricRatesTestData))]
        public async void GetConversionRateHistoricDataAsync_ReturnsCorrectResult(string from, string to,
            decimal? expectedRate, ErrorCode resultErrorCode, ErrorCode dailyRateErrorCode)
        {
            var result = _service.GetConversionRateHistoricDataAsync(from, to);

            Assert.IsType<ConversionRateHistoryDto>(result);
            Assert.Equal(resultErrorCode, result.ErrorCode);
            foreach (var dailyConversionRate in result.DailyConversionRates)
            {
                Assert.Equal(expectedRate, dailyConversionRate.Rate);
                Assert.Equal(dailyRateErrorCode, dailyConversionRate.ErrorCode);
            }
        }

        public static IEnumerable<object[]> CurrencyConversionRateTestData =>
            new List<object[]>
            {
                new object[] {"EUR", "EUR", "2020-doesn't-matter", new decimal(1), null},
                new object[] {"EUR", "DOESN'T EXIST", "2020-08-01", null, ErrorCode.RateNotFound},
                new object[] {"DOESN'T EXIST", "EUR", "2020-doesnt-matter", null, ErrorCode.RateNotFound},
                new object[] {"", "", "2020-doesn't-matter", null, ErrorCode.CurrencyNotProvided},
                new object[] {"EUR", "", "2020-doesn't-matter", null, ErrorCode.CurrencyNotProvided}
            };

        public static IEnumerable<object[]> CurrencyConversionHistoricRatesTestData =>
            new List<object[]>
            {
                new object[] {"EUR", "EUR", new decimal(1), null, null},
                new object[] {"EUR", "DOESN'T EXIST", new decimal(0), null, ErrorCode.RateNotFound},
                new object[] {"DOESN'T EXIST", "EUR", new decimal(0), null, ErrorCode.RateNotFound},
                new object[] {"", "", null, ErrorCode.CurrencyNotProvided, null},
            };
    }
}