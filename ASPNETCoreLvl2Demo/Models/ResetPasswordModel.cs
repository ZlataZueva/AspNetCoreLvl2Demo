using System.ComponentModel.DataAnnotations;

namespace ASPNETCoreLvl2Demo.Models
{
    public class ResetPasswordModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword  { get; set; }
    }
}