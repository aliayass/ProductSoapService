using System.ComponentModel.DataAnnotations;

namespace ProductSoapService.Dtos
{
    public class ProductDto
    {
        public int? id { get; set; }
        public string? itemId { get; set; } = string.Empty;
        public string? beden { get; set; } = string.Empty;
        public string? barkod { get; set; } = string.Empty;
        public string? renk { get; set; } = string.Empty;
        public decimal? price { get; set; }
    }
}
