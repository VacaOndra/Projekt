using Pojisteni.Data;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Pojisteni.Models
{
    public class Insurance
    {
        [Key]
        public int ID { get; set; }


        [Required(ErrorMessage = "Vyplňte název")]
        [Display(Name = "Název pojistky")]
        public string Name { get; set; }
        public virtual ICollection<InsuranceInsured>? InsuranceInsureds { get; set; }
    }
}
