using CurrencyExchange.Services.Enums;

namespace CurrencyExchange.Services.Dto
{
    public class CurrencyConversionRateResponseDto
    {
        public decimal? ConversionRate { get; set; }
        public ErrorCode ErrorCode { get; set; }
    }
}
