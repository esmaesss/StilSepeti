using System.ComponentModel.DataAnnotations;
using StilSepetiApp.Enums;

namespace StilSepetiApp.DTO
{
    public class Userdto
    {
        [Required(ErrorMessage = "Kullanıcı ID zorunludur.")]
        public int userId { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;

        public Role Role { get; set; } = Role.Member;
    }
}
