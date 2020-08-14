using System.Collections.Generic;
using CurrencyExchange.Services.Enums;

namespace CurrencyExchange.Services.Dto
{
    public class ConversionRateHistoryDto
    {
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public IEnumerable<DailyConversionRate> DailyConversionRates { get; set; } = new List<DailyConversionRate>();
        public ErrorCode ErrorCode { get; set; }
    }
}
