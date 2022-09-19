using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Pojisteni.Models
{
    public class RegisterInsuredViewModel
    {

        [Key]
        [Required]
        public int InsuredID { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} musí mít délku alespoň {2} a nejvíc {1} znaků.", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "Heslo")]
        public string Password { get; set; } = "";

        public virtual Insured? Insured { get; set; }
    }
}
