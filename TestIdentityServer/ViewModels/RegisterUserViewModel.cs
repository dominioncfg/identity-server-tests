using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TestIdentityServer.ViewModels
{
    public class RegisterUserViewModel
    {
        [Required]
        [MaxLength(200)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(200)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [MaxLength(250)]
        [Display(Name = "Given name")]
        public string GivenName { get; set; }

        [Required]
        [MaxLength(250)]
        [Display(Name = "Family name")]
        public string FamilyName { get; set; }

        [Required]
        [MaxLength(250)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Province")]
        public int ProvinceId { get; set; }

        [Required]
        [Display(Name = "Age")]
        public int Age { get; set; }

        public SelectList Provinces { get; set; } =
            new SelectList(
                new[] 
                {
                    new {Id= 01,Value = "Pinar Del Rio"},
                    new {Id= 02,Value = "Artemisa"},
                    new {Id= 03,Value = "La Habana"},
                    new {Id= 04,Value = "Mayabeque"},
                    new {Id= 05,Value = "Matanzas "},
                    new {Id= 06,Value = "Cienfuegos"},
                    new {Id= 07,Value = "Villa Clara"},
                    new {Id= 08,Value = "Sancti Spíritus"},
                    new {Id= 09,Value = "Ciego De Ávila"},
                    new {Id= 10,Value = "Camaguey"},
                    new {Id= 11,Value = "Las Tunas"},
                    new {Id= 12,Value = "Granma"},
                    new {Id= 13,Value = "Holguin"},
                    new {Id= 14,Value = "Santiago De Cuba"},
                    new {Id= 15,Value = "Guantanamo"},
                    new {Id= 16,Value = "Isla De La Juventud"},
                },
                "Id",
                "Value");

        public string ReturnUrl { get; set; }

    }
}
