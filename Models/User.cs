using System.ComponentModel.DataAnnotations;
using StilSepetiApp.Enums;
using System.Text.Json.Serialization;


namespace StilSepetiApp.Models
{
    public class User
    {
        public int userId { get; set; }
        [Required]
        public string Username { get; set; } = null!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        public Role Role { get; set; } = Role.Member;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        [JsonIgnore]
        public ICollection<ReturnRequest> ReturnRequests { get; set; } = new List<ReturnRequest>();
    }


}


