using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pojisteni.Models
{
    public class Event
    {
        [Key]
        public int ID { get; set; }

        public int InsuranceInsuredID { get; set; }
        public virtual InsuranceInsured? InsuranceInsured { get; set; }

        [Required(ErrorMessage = "Vyplňte popis")]
        [Display(Name = "Popis")]
        public string Description { get; set; }


        [Display(Name = "Datum vzniku")]
        public DateTime DateTime { get; set; }

        [Display(Name = "Datum vložení")]
        public DateTime InsertDateTime { get; set; }

        [Display(Name = "Vyřízeno")]
        public bool Settled { get; set; }
    }
}
