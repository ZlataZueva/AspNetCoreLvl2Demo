using System.ComponentModel.DataAnnotations;

namespace ASPNETCoreLvl2Demo.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        public string Name { get; set; }
    }
}