using System.ComponentModel.DataAnnotations;

namespace ASPNETCoreLvl2Demo.Models
{
    public class RegisterModel
    {
        [Required]
        public string Name { get; set; }

        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        [Compare(nameof(Password))]
        [DataType(DataType.Password)]
        [Required]
        public string ConfirmPassword { get; set; }

        public string FavoriteMusician { get; set; }
    }
}