
using System.ComponentModel.DataAnnotations;
using StilSepetiApp.Models;

namespace StilSepetiApp.Models

{
    public class CartItem
    {
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        public User? User { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        public Product? Product { get; set; }
        public int ProductId { get; set; }

        [Required]
        public string SelectedSize { get; set; } = null!; // XS, S, M, L, XL


    }
}
