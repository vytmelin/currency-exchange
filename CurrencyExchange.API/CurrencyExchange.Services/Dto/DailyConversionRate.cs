using CurrencyExchange.Services.Enums;

namespace CurrencyExchange.Services.Dto
{
    public class DailyConversionRate
    {
        public string Date { get; set; }
        public decimal? Rate { get; set; }
        public ErrorCode ErrorCode { get; set; }
    }
}
