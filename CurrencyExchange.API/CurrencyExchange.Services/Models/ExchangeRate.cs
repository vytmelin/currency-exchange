using System.ComponentModel.DataAnnotations.Schema;

namespace CurrencyExchange.Services.Models
{
    public class ExchangeRate
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Date { get; set; }
        public string Currency { get; set; }
        public decimal Rate { get; set; }
    }
}
