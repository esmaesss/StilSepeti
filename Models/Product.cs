using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace StilSepetiApp.Models
{
    public class Product
    {
        public int Id { get; set; }  
        public string Name { get; set; } = null!;
        public string?Description { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string SubCategory { get; set; } = null!;
        public int Stock { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } 

        public string? Colour { get; set; }

        public string Brand { get; set; } = null!;

        public string Size { get; set; } = null!;
        public int SellerId { get; set; }

        [JsonIgnore]
        public User? Seller { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime LastUpdatedAt { get; set; }
    }
}
