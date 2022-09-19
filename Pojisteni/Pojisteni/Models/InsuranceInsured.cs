using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Pojisteni.Models
{
    public class InsuranceInsured
    {
        [Key]
        public int ID { get; set; }
        public int InsuredID { get; set; }

        [Display(Name = "Pojištěný")]
        public virtual Insured? Insured { get; set; }

        public int InsuranceID { get; set; }

        [Display(Name = "Pojistka")]
        public virtual Insurance? Insurance { get; set; }


        [Display(Name = "Částka")]
        [DataType(DataType.Currency)]
        [Range(100000, 100000000, ErrorMessage = "Zadejte prosím částku od {1:C0} do {2:C0}.")]
        public int Amount { get; set; }


        [Required(ErrorMessage = "Povinné pole")]
        [Display(Name = "Předmět pojištění")]
        public string Subject { get; set; }


        [Display(Name = "Platnost od")]
        public DateTime StartsAt { get; set; }


        [Display(Name = "Platnost do")]
        public DateTime EndsAt { get; set; }

        public virtual bool Status => this.EndsAt.Date < DateTime.Now.Date;
        public virtual string StatusString => Status ? "Neaktivní" : "Aktivní";
    }
}
