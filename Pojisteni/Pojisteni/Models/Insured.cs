using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Pojisteni.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Pojisteni.Models
{
    public class Insured
    {
        [Key]
        public int ID { get; set; }


        [Required(ErrorMessage = "Povinné pole")]
        [Display(Name="Jméno")]
        public string Name { get; set; }


        [Required(ErrorMessage = "Povinné pole")]
        [Display(Name = "Příjmení")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Povinné pole")]
        [EmailAddress(ErrorMessage = "Neplatná emailová adresa")]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Povinné pole")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "{0} musí mít {1} čísel (bez mezer a bez předvolby)")]
        [Display(Name = "Mobil")]
        public string PhoneNumber { get; set; }


        [StringLength(10, ErrorMessage = "{0} musí mít délku alespoň {2} a nejvíc {1} čísel.", MinimumLength = 9)]
        [Required(ErrorMessage = "Vypňte rodné číslo")]
        [Display(Name = "Rodné číslo")]
        public string IdentityNumber { get; set; }


        [Required(ErrorMessage = "Povinné pole")]
        [Display(Name = "Adresa")]
        public string Adress { get; set; }


        [Required(ErrorMessage = "Povinné pole")]
        [Display(Name = "Město")]
        public string City { get; set; }


        [Required(ErrorMessage = "Povinné pole")]
        [StringLength(5,MinimumLength = 5, ErrorMessage = "{0} musí mít {1} čísel")]
        [Display(Name = "PSČ")]
        public string PostalCode { get; set; }

        public virtual ICollection<InsuranceInsured>? InsuranceInsureds { get; set; }

        [Display(Name = "Příjmení a jméno")]
        public virtual string LastNameAndName => $"{LastName} {Name}";

        [Display(Name = "Adresa")]
        public virtual string FullAdress => $"{Adress}, {City}";
    }
}
