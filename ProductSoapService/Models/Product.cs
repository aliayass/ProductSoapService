using System.ComponentModel.DataAnnotations;

namespace ProductSoapService.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string ItemId { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Beden { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Barkod { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Renk { get; set; } = string.Empty;
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
    }
}
