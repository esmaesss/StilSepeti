using System.ComponentModel.DataAnnotations;

namespace StilSepetiApp.Enums
{
    public enum Role
    {
        
        [Display(Name = "Üye")]
        Member,

        [Display(Name = "Satıcı")]
        Seller,

        [Display(Name = "Yönetici")]
        Admin
    }

}

