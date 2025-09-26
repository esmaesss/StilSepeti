using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Json.Serialization;
using StilSepetiApp.Enums;



namespace StilSepetiApp.Models
{
  
    public class ReturnRequest
    {

        public int Id { get; set; }
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public Order? Order { get; set; }
        [Required]
        public string Reason { get; set; } = null!;
        [Required]
        public string? RejectionReason { get; set; } 
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public ReturnStatus Status { get; set; } = ReturnStatus.Requested;

        [JsonIgnore]
        public User? User { get; set; }
        public DateTime? ReviewedAt { get; set; }
    }
}
