using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Pojisteni.Models
{
    public class ChangePasswordViewModel
    {

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Staré heslo")]
        public string OldPassword { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Nové heslo")]
        public string Password { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Potvrzení nového hesla")]
        [Compare("Password", ErrorMessage = "Zadaná hesla se musí shodovat.")]
        public string ConfirmPassword { get; set; } = "";
    }
}
